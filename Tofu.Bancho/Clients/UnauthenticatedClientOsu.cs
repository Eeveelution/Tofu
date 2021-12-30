using System;
using System.IO;
using System.Net.Sockets;
using EeveeTools.Database;
using MySqlConnector;
using Tofu.Bancho.Clients.OsuClients;
using Tofu.Bancho.Packets.Build282;

namespace Tofu.Bancho.Clients {
    /// <summary>
    /// A Unauthenticated ClientOsu
    /// </summary>
    public class UnauthenticatedClientOsu : ClientOsu {
        private ClientType _clientType;

        public UnauthenticatedClientOsu(Bancho bancho, TcpClient client) : base(bancho, client) {}

        /// <summary>
        /// Used for Handling everything client related, this gets called by TofuWorkers
        /// </summary>
        public override void HandleClient() {}
        /// <summary>
        /// Handles authentication
        /// </summary>
        /// <returns></returns>
        public override bool Authenticate() {
            string username;
            string password;
            string clientInfo;

            StreamReader reader = new StreamReader(this.ClientStream);

            if (this.ClientStream.DataAvailable && this.ClientStream.CanRead) {
                //Read the Login String
                username = reader.ReadLine();
                password = reader.ReadLine();
                clientInfo = reader.ReadLine();

                string[] split = clientInfo.Split('|');

                //Determine the Version
                string version = split[0];

                this.ClientInformation = new ClientInformation {
                    Username = username
                };

                switch (version) {
                    case "b282":
                        this._clientType = ClientType.Build282;

                        try {
                            this.ClientInformation.Timezone = int.Parse(split[1]);
                        } catch {
                            this.Kill("Malformed Login String.");
                        }

                        break;
                }

                const string loginSql = "SELECT user_id, username, password FROM users WHERE username=@username";

                MySqlParameter[] loginParams = new[] {
                    new MySqlParameter("@username", username)
                };

                var databaseResults = MySqlDatabaseHandler.MySqlQuery(this.Bancho.DatabaseContext, loginSql, loginParams);
                var result = databaseResults[0];

                int userId = (int) result["user_id"];
                string dbPassword = (string) result["password"];

                this.ClientInformation.Id = userId;

                //TODO: BCrypt
                if (password != dbPassword) {
                    new BanchoLoginResponse(-1).ToPacket().Send(this.ClientStream);

                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Kills the Client
        /// </summary>
        /// <param name="reason">Possible reason</param>
        public override void Kill(string reason) {
            this.TcpClient.Close();
            this.ClientStream.Close();
        }

        /// <summary>
        /// Method unique to this ClientOsu, this upgrades the Connection
        /// </summary>
        /// <returns>A Upgraded ClientOsu</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ClientOsu ToClientOsu() {
            return this._clientType switch {
                ClientType.Build282 => new ClientBuild282(this.Bancho, this.TcpClient, this.ClientInformation),
                ClientType.Irc      => null,
                _                   => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
