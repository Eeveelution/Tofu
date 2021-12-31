using System;
using System.IO;
using System.Net.Sockets;
using Tofu.Bancho.DatabaseObjects;
using Tofu.Bancho.PacketObjects;
using Tofu.Bancho.PacketObjects.Enums;
using Tofu.Bancho.Packets.Build282.Enums;
using Tofu.Bancho.Packets.Common;

namespace Tofu.Bancho.Clients.OsuClients {
    /// <summary>
    /// A Unauthenticated ClientOsu
    /// </summary>
    public class UnknownClientOsu {
        public readonly TcpClient     Client;
        public readonly NetworkStream ClientStream;

        /// <summary>
        /// Everything known about the User
        /// </summary>
        public User User;
        /// <summary>
        /// All the Client Data we can get from the Login request
        /// </summary>
        public ClientData ClientData;

        public UnknownClientOsu(TcpClient client) {
            this.Client       = client;
            this.ClientStream = client.GetStream();
        }

        /// <summary>
        /// Handles authentication
        /// </summary>
        /// <returns></returns>
        public bool PerformAuth() {
            string username;
            string password;
            string clientInfo;

            StreamReader reader = new StreamReader(this.ClientStream);

            if (this.ClientStream.DataAvailable && this.ClientStream.CanRead) {
                //Read the Login String
                username = reader.ReadLine();
                password = reader.ReadLine();
                clientInfo = reader.ReadLine();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(clientInfo)) {
                    this.Kill();
                    return false;
                }

                string[] split = clientInfo.Split('|');

                //Determine the Version
                string version = split[0];

                switch (version) {
                    case "b282":
                        this.ClientData.ClientType = ClientType.Build282;

                        try {
                            this.ClientData.Timezone = byte.Parse(split[1]);
                        } catch {
                            this.Kill();
                            return false;
                        }

                        break;
                }

               User databaseUser = User.FromDatabase(username);

               this.User = databaseUser;

               if (databaseUser == null) {
                    this.SendLoginResponse(LoginResult.AuthFailed);
                    return false;
               }

               if (databaseUser.Password != password) {
                   this.SendLoginResponse(LoginResult.AuthFailed);
                   return false;
               }

               if (databaseUser.Banned) {
                   this.SendLoginResponse(LoginResult.Banned);
                   return false;
               }

               return true;
            }

            return false;
        }

        private void SendLoginResponse(LoginResult result) {
            switch (this.ClientData.ClientType) {
                case ClientType.Build282: {
                    //If the Login was successful while in Unauthenticated
                    switch (result) {
                        case LoginResult.AuthFailed:
                        case LoginResult.Unauthorized:
                        case LoginResult.Banned:
                        case LoginResult.ServerFailure:
                            this.SendLoginResponse(-1);
                            return;
                        case LoginResult.VersionMismatch:
                            this.SendLoginResponse(-2);
                            return;
                        default:
                            this.SendLoginResponse((int) this.User.Id);
                            this.User.FetchAllStats();
                            break;
                    }
                    break;
                }
            }
        }

        private void SendLoginResponse(int id) {
            BanchoLoginResponse response = new BanchoLoginResponse(id);

            switch (this.ClientData.ClientType) {
                case ClientType.Build282:
                    response.ToPacket().Send(this.ClientStream, false);
                    break;
                default:
                    response.ToPacket().Send(this.ClientStream);
                    break;
            }
        }

        /// <summary>
        /// Kills the Client
        /// </summary>
        public void Kill() {
            this.Client.Close();
            this.ClientStream.Close();
        }

        /// <summary>
        /// Method unique to this ClientOsu, this upgrades the Connection
        /// </summary>
        /// <returns>A Upgraded ClientOsu</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ClientOsu ToClientOsu() {
            return this.ClientData.ClientType switch {
                ClientType.Build282 => new ClientBuild282(this),
                ClientType.Irc      => null,
                _                   => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
