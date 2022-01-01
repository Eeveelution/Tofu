using System;
using System.IO;
using EeveeTools.Helpers;
using Kettu;
using Tofu.Bancho.Logging;
using Tofu.Bancho.PacketObjects;
using Tofu.Bancho.Packets;
using Tofu.Bancho.Packets.Build282;
using Tofu.Bancho.Packets.Build282.Enums;
using Tofu.Bancho.Packets.Common;
using Tofu.Common;

namespace Tofu.Bancho.Clients.OsuClients {
    public class ClientBuild282 : ClientOsu {
        /// <summary>
        /// Creates a b282 osu! Client
        /// </summary>
        /// <param name="clientOsu">UnknownClientOsu to pull data from</param>
        public ClientBuild282(UnknownClientOsu clientOsu) : base(clientOsu.Client) {
            this.User       = clientOsu.User;
            this.ClientData = clientOsu.ClientData;
            this.Presence   = new OsuPresence();
        }
        /// <summary>
        /// Handles the client
        /// </summary>
        public override void HandleClient() {
            //Handle Timing out the Client
            if (this.LastPongTime + PongTimeout <= DateTime.Now && this.LastPongTime != DateTime.MinValue) {
                this.Kill("Client timed out.");
                return;
            }

            //Check if there's something available to read
            if (this.TcpClient.Available != 0 && this.ClientStream.DataAvailable && this.ClientStream.CanRead) {
                //Read
                byte[] buffer = new byte[4096];
                int recieved = this.ClientStream.Read(buffer, 0, 4096);

                //Cut out unnecessary part
                byte[] readBuffer = new byte[recieved];
                Buffer.BlockCopy(buffer, 0, readBuffer, 0, recieved);

                //Handle
                this.HandleIncoming(readBuffer);
            }

            //Sends all queued up packets, also performs pings
            this.SendOutgoing();
        }

        /// <summary>
        /// Handles incoming data
        /// </summary>
        /// <param name="data">Incoming Data</param>
        private void HandleIncoming(byte[] data) {
            //Set the Last Pong time to now, as we've just recieved something from the client
            this.LastPongTime = DateTime.Now;

            using MemoryStream dataStream = new MemoryStream(data);
            using BinaryReader dataReader = new BinaryReader(dataStream);

            //Read while the Stream isn't over
            while (dataReader.BaseStream.Position < dataReader.BaseStream.Length) {
                try {
                    //Read Header
                    RequestType requestType = (RequestType) dataReader.ReadUInt16();

                    //Log if it's not a pong
                    if (requestType != RequestType.OsuPong)
                        Logger.Log($"[b282] <{this.Username}@{this.Id}> Got Packet {requestType}", LoggerLevelInfo.Instance);

                    int packetLength = dataReader.ReadInt32();
                    byte[] packetBytes = dataReader.ReadBytes(packetLength);

                    using MemoryStream stream = new MemoryStream(packetBytes);
                    using BanchoReader reader = new BanchoReader(stream);

                    reader.BaseStream.Position = 0;

                    if (reader.BaseStream.Position != 0) {
                        Console.WriteLine("|OIJUDSHFiuDSHGbf;ldsBGhfkDSBVLJHFDSGHLOFSHGFV");
                    }

                    //Handle Packets
                    switch (requestType) {
                        case RequestType.OsuSendUserStatus: {
                            OsuSendUserStatus status = new OsuSendUserStatus(stream);

                            this.Presence.StatusText      = status.StatusText;
                            this.Presence.BeatmapChecksum = status.BeatmapChecksum;
                            this.Presence.UserStatus      = status.Status;
                            this.Presence.EnabledMods     = status.CurrentMods;

                            Global.Bancho.ClientManager.BroadcastPacketOsu(client => client.HandleOsuUpdate(this));

                            break;
                        }
                        case RequestType.OsuExit: {
                            Global.Bancho.ClientManager.BroadcastPacketOsu(clientOsu => clientOsu.HandleOsuQuit(this));

                            this.Kill("Client exited.");

                            break;
                        }
                        case RequestType.OsuRequestStatusUpdate: {
                            foreach (ClientOsu client in Global.Bancho.ClientManager.OsuClients) {
                                this.HandleOsuUpdate(client);
                            }

                            break;
                        }
                        case RequestType.OsuSendIrcMessage: {
                            Message message = new Message {
                                Sender = this.Username,
                                Content = reader.ReadString()
                            };

                            Global.Bancho.ClientManager.BroadcastPacketOsuExceptSelf(clientOsu => clientOsu.SendIrcMessage(message), this);

                            break;
                        }
                    }
                }
                catch (Exception e) {

                }
            }
        }

        /// <summary>
        /// Sends all outgoing queued Packets out
        /// </summary>
        private void SendOutgoing() {
            try {
                //While there's more packets to send
                while(this.PacketQueue.TryDequeue(out Packet packet)) {
                    if((RequestType)packet.PacketId != RequestType.BanchoPing)
                        Logger.Log($"[b282] <{this.Username}@{this.Id}> Sending { (RequestType) packet.PacketId }", LoggerLevelInfo.Instance);

                    //Send
                    packet.Send(this.ClientStream, false);
                    this.ClientStream.Flush();

                    //Set the last ping time to now, as the client doesn't care what arrives, as long as something arrives
                    this.LastPingTime = DateTime.Now;
                }

                //If we've exceeded 10 seconds without sending anything, send a ping
                if (this.LastPingTime + PingTimeout <= DateTime.Now) {
                    this.Ping();
                    this.LastPingTime = DateTime.Now;
                }
            }
            catch { /* */ }
        }

        /// <summary>
        /// Handles killing the client
        /// </summary>
        /// <param name="reason">Potential Reason</param>
        public override void Kill(string reason) {
            Logger.Log($"[b282] <{this.Username}@{this.Id}> Killed for: {reason}", LoggerLevelInfo.Instance);

            Global.Bancho.ClientManager.RemoveClient(this);
        }

        /// <summary>
        /// Gets called upon successful Registration
        /// </summary>
        public override void RegistrationComplete() {
            Global.Bancho.ClientManager.BroadcastPacketOsu(client => client.HandleOsuUpdate(this));

            foreach (ClientOsu client in Global.Bancho.ClientManager.OsuClients) {
                this.HandleOsuUpdate(client);
            }

            this.SendIrcMessage("Welcome to Tofu!Bancho!");
        }

        #region Packets

        /// <summary>
        /// Sends a BanchoLoginResponse
        /// </summary>
        /// <param name="userId">User ID to send</param>
        public override void LoginResponse(int userId) => this.QueuePacket(new BanchoLoginResponse(userId));
        /// <summary>
        /// Sends a BanchoPing
        /// </summary>
        public override void Ping() => this.QueuePacket(new BanchoPing());
        /// <summary>
        /// Sends a BanchoHandleOsuUpdate
        /// <param name="clientOsu">ClientOsu to inform this client about</param>
        /// </summary>
        public override void HandleOsuUpdate(ClientOsu clientOsu) => this.QueuePacket(BanchoHandleOsuUpdate.Create(clientOsu));
        /// <summary>
        /// Sends a BanchoSendIrcMessage
        /// </summary>
        /// <param name="message">Message to send</param>
        public override void SendIrcMessage(Message message) => this.QueuePacket(BanchoSendIrcMessage.Create(message));
        /// <summary>
        /// Sends a BanchoSendIrcMessage
        /// </summary>
        /// <param name="message">Message to send</param>
        public override void SendIrcMessage(string message) => this.QueuePacket(BanchoSendIrcMessage.Create(new Message {
            Content = message,
            Sender = "TofuBot",
        }));
        /// <summary>
        /// Sends a BanchoHandleOsuQuit
        /// </summary>
        /// <param name="clientOsu">ClientOsu that quit</param>
        public override void HandleOsuQuit(ClientOsu clientOsu) => this.QueuePacket(BanchoHandleOsuQuit.Create(clientOsu));

        #endregion

    }
}
