using System;
using System.IO;
using System.Net.Sockets;
using EeveeTools.Database;
using MySqlConnector;
using Tofu.Bancho.DatabaseObjects;
using Tofu.Bancho.Helpers;

namespace Tofu.Bancho.Clients.OsuClients {
    /// <summary>
    /// A Unauthenticated ClientOsu
    /// </summary>
    public class UnknownClientOsu {
        private ClientType _clientType;

        private TcpClient     _client;
        private NetworkStream _clientStream;

        private ClientInformation _clientInformation;

        public UnknownClientOsu(TcpClient client) {
            this._client       = client;
            this._clientStream = client.GetStream();
        }

        /// <summary>
        /// Used for Handling everything client related, this gets called by TofuWorkers
        /// </summary>
        public void HandleClient() {}
        /// <summary>
        /// Handles authentication
        /// </summary>
        /// <returns></returns>
        public bool PerformAuth() {
            string username;
            string password;
            string clientInfo;

            StreamReader reader = new StreamReader(this._clientStream);

            if (this._clientStream.DataAvailable && this._clientStream.CanRead) {
                //Read the Login String
                username = reader.ReadLine();
                password = reader.ReadLine();
                clientInfo = reader.ReadLine();

                string[] split = clientInfo.Split('|');

                //Determine the Version
                string version = split[0];

                switch (version) {
                    case "b282":
                        this._clientType = ClientType.Build282;

                        try {
                            this._clientInformation.Timezone = int.Parse(split[1]);
                        } catch {
                            this.Kill();
                        }

                        break;
                }

                const string loginSql = "SELECT * FROM users WHERE username=@username";

                MySqlParameter[] loginParams = new[] {
                    new MySqlParameter("@username", username)
                };

                var databaseResults = MySqlDatabaseHandler.MySqlQuery(Global.DatabaseContext, loginSql, loginParams);

                if (databaseResults.Length == 0)
                    return false;

                var result = databaseResults[0];

                User databaseUser = new User();
                databaseUser.MapDatabaseResults(result);

                this._clientInformation.User = databaseUser;
            }

            return true;
        }
        /// <summary>
        /// Kills the Client
        /// </summary>
        /// <param name="reason">Possible reason</param>
        public void Kill() {
            this._client.Close();
            this._clientStream.Close();
        }

        /// <summary>
        /// Method unique to this ClientOsu, this upgrades the Connection
        /// </summary>
        /// <returns>A Upgraded ClientOsu</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ClientOsu ToClientOsu() {
            return this._clientType switch {
                ClientType.Build282 => new ClientBuild282(this._client, this._clientInformation),
                ClientType.Irc      => null,
                _                   => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
