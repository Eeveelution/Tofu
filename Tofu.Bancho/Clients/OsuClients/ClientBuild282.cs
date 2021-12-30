using System;
using System.IO;
using System.Net.Sockets;
using EeveeTools.Helpers;
using Kettu;
using Tofu.Bancho.Logging;
using Tofu.Bancho.PacketObjects.Enums;
using Tofu.Bancho.Packets;
using Tofu.Bancho.Packets.Build282.Enums;
using Tofu.Bancho.Packets.Common;

namespace Tofu.Bancho.Clients.OsuClients {
    public class ClientBuild282 : ClientOsu {
        /// <summary>
        /// Creates a b282 osu! Client
        /// </summary>
        /// <param name="client">The TCP Socket it's using</param>
        /// <param name="information">The currently known information about the client</param>
        public ClientBuild282(TcpClient client, ClientInformation information) : base(client) {
            this.ClientInformation = information;
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
            using BanchoReader dataReader = new BanchoReader(dataStream);

            //Read while the Stream isn't over
            while (dataReader.BaseStream.Position < dataReader.BaseStream.Length) {
                //Read Header
                RequestType requestType = (RequestType) dataReader.ReadUInt16();

                //Log if it's not a pong
                if(requestType != RequestType.OsuPong)
                    Logger.Log($"[b282] <{this.ClientInformation.Username}@{this.ClientInformation.Id}> Got Packet {requestType}", LoggerLevelInfo.Instance);

                int packetLength = dataReader.ReadInt32();

                using MemoryStream stream = new MemoryStream(dataReader.ReadBytes(packetLength));
                using BanchoReader reader = new BanchoReader(stream);

                //Handle Packets
                switch (requestType) {
                    case RequestType.OsuExit:
                        this.Kill("Client exited.");
                        break;
                    case RequestType.OsuRequestStatusUpdate:
                        break;
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
                        Logger.Log($"[b282] <{this.ClientInformation.Username}@{this.ClientInformation.Id}> Sending { (RequestType) packet.PacketId }", LoggerLevelInfo.Instance);

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
            Logger.Log($"[b282] <{this.ClientInformation.Username}@{this.ClientInformation.Id}> Killed for: {reason}", LoggerLevelInfo.Instance);

            Global.Bancho.ClientManager.RemoveClient(this);
        }

        /// <summary>
        /// Gets called upon successful Registration
        /// </summary>
        public override void RegistrationComplete() {
            //If the Login was successful while in Unauthenticated
            switch (this.ClientInformation.PendingLoginResult) {
                case LoginResult.AuthFailed:
                case LoginResult.Unauthorized:
                case LoginResult.Banned:
                case LoginResult.ServerFailure:
                    this.LoginResponse(-1);
                    break;
                case LoginResult.VersionMismatch:
                    this.LoginResponse(-2);
                    break;
                default:
                    this.LoginResponse(this.ClientInformation.Id);
                    break;
            }

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

        #endregion

    }
}
