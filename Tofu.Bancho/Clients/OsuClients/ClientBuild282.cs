using System;
using System.IO;
using System.Net.Sockets;
using EeveeTools.Helpers;
using Kettu;
using Tofu.Bancho.Logging;
using Tofu.Bancho.Packets;
using Tofu.Bancho.Packets.Build282;

namespace Tofu.Bancho.Clients.OsuClients {
    public class ClientBuild282 : ClientOsu {

        public ClientBuild282(Bancho bancho, TcpClient client, ClientInformation information) : base(bancho, client) {
            this.ClientInformation = information;
        }

        public override void HandleClient() {
            if (this.LastPongTime + PongTimeout <= DateTime.Now && this.LastPongTime != DateTime.MinValue) {
                this.Kill("Client timed out.");
                return;
            }

            if (this.TcpClient.Available != 0 && this.ClientStream.DataAvailable && this.ClientStream.CanRead) {
                byte[] buffer = new byte[4096];
                int recieved = this.ClientStream.Read(buffer, 0, 4096);

                byte[] readBuffer = new byte[recieved];
                Buffer.BlockCopy(buffer, 0, readBuffer, 0, recieved);

                this.HandleIncoming(readBuffer);

            }

            this.SendOutgoing();
        }

        private void HandleIncoming(byte[] data) {
            this.LastPongTime = DateTime.Now;

            using MemoryStream dataStream = new MemoryStream(data);
            using BanchoReader dataReader = new BanchoReader(dataStream);

            while (dataReader.BaseStream.Position < dataReader.BaseStream.Length) {
                RequestType requestType = (RequestType) dataReader.ReadUInt16();
                int packetLength = dataReader.ReadInt32();

                using MemoryStream stream = new MemoryStream(dataReader.ReadBytes(packetLength));
                using BanchoReader reader = new BanchoReader(stream);

                if(requestType != RequestType.OsuPong)
                    Logger.Log($"[b282] <{this.ClientInformation.Username}@{this.ClientInformation.Id}> Got Packet {requestType} with length {packetLength}", LoggerLevelInfo.Instance);

                switch (requestType) {
                    case RequestType.OsuExit:
                        this.Kill("Client exited.");
                        break;
                }
            }
        }

        private void SendOutgoing() {
            try {
                Packet packet;

                while(this.PacketQueue.TryDequeue(out packet)) {
                    Logger.Log($"[b282] <{this.ClientInformation.Username}@{this.ClientInformation.Id}> Sending { (RequestType) packet.PacketId }", LoggerLevelInfo.Instance);

                    packet.Send(this.ClientStream, false);

                    this.ClientStream.Flush();

                    this.LastPingTime = DateTime.Now;
                }

                if (this.LastPingTime + PingTimeout <= DateTime.Now) {
                    this.SendPing();
                    this.LastPingTime = DateTime.Now;
                }
            }
            catch {

            }

        }

        public override void Kill(string reason) {
            Logger.Log($"[b282] <{this.ClientInformation.Username}@{this.ClientInformation.Id}> Killed for: {reason}", LoggerLevelInfo.Instance);


            this.Bancho.ClientManager.RemoveClient(this);
        }

        public override void RegistrationComplete() {
            if (this.ClientInformation.LoginSuccessPending)
                this.SendLoginResponse(this.ClientInformation.Id);

        }

        #region Packets

        public void SendLoginResponse(int userId) => this.PacketQueue.Enqueue(new BanchoLoginResponse(userId));
        public void SendPing() => this.PacketQueue.Enqueue(new BanchoPing());

        #endregion

    }
}
