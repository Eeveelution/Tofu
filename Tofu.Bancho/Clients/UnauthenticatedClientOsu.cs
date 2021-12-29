using System;
using System.IO;
using System.Net.Sockets;
using Tofu.Bancho.Clients.OsuClients;

namespace Tofu.Bancho.Clients {
    public class UnauthenticatedClientOsu : ClientOsu {
        private ClientType _clientType;

        public UnauthenticatedClientOsu(Bancho bancho, TcpClient client) : base(bancho, client) {
        }

        public override void HandleClient() {

        }

        public override bool Authenticate() {
            string username;
            string password;
            string clientInfo;

            StreamReader reader = new StreamReader(this.ClientStream);

            if (this.ClientStream.DataAvailable && this.ClientStream.CanRead) {
                username = reader.ReadLine();
                password = reader.ReadLine();
                clientInfo = reader.ReadLine();

                string version = clientInfo.Split("|")[0];

                switch (version) {
                    case "b282":
                        this._clientType = ClientType.Build282;
                        break;
                }

                this.ClientInformation = new ClientInformation {
                    Username = username, Id = 1, LoginSuccessPending = true
                };
            }

            return true;
        }

        public override void Kill(string reason) {
            this.TcpClient.Close();
            this.ClientStream.Close();
        }

        public ClientOsu ToClientOsu() {
            return this._clientType switch {
                ClientType.Build282 => new ClientBuild282(this.Bancho, this.TcpClient, this.ClientInformation),
                ClientType.Irc      => null,
                _                   => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
