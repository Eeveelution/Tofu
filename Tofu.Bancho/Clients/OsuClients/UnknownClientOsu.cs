using System;
using System.IO;
using System.Net.Sockets;
using Tofu.Bancho.DatabaseObjects;
using Tofu.Bancho.PacketObjects;
using Tofu.Bancho.PacketObjects.Enums;
using Tofu.Bancho.Packets.Build282.Enums;

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
        /// Handles authentication
        /// </summary>
        /// <returns></returns>
        public void PerformAuth() {
            this._clientInformation = new ClientInformation {
                CurrentPlayMode = 0,
                Presence = new OsuPresence {
                    BeatmapChecksum = "",
                    StatusText = "",
                    EnabledMods = Mods.None,
                    UserStatus = Status.Idle
                }
            };

            string username;
            string password;
            string clientInfo;

            StreamReader reader = new StreamReader(this._clientStream);

            if (this._clientStream.DataAvailable && this._clientStream.CanRead) {
                //Read the Login String
                username = reader.ReadLine();
                password = reader.ReadLine();
                clientInfo = reader.ReadLine();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(clientInfo)) {
                    this.Kill();
                    return;
                }

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
                            return;
                        }

                        break;
                }

               User databaseUser = User.FromDatabase(username);

               this._clientInformation.User = databaseUser;

               if (databaseUser == null) {
                   this._clientInformation.PendingLoginResult = LoginResult.AuthFailed;
                   this._clientInformation.User = new User {
                       Username = username, Id = -1
                   };
                   return;
               }

               if (databaseUser.Password != password) {
                   this._clientInformation.PendingLoginResult = LoginResult.AuthFailed;
                   this._clientInformation.User = new User {
                       Username = username, Id = -1
                   };
                   return;
               }
            }
        }
        /// <summary>
        /// Kills the Client
        /// </summary>
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
