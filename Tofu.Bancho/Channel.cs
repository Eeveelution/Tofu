using System;
using System.Collections.Generic;
using EeveeTools.Database;
using MySqlConnector;
using Tofu.Bancho.Clients;
using Tofu.Bancho.Clients.OsuClients;
using Tofu.Bancho.Helpers;
using Tofu.Bancho.PacketObjects;
using Tofu.Common;

namespace Tofu.Bancho {
    public class Channel {
        public string Name { get; set; }
        public string Topic { get; set; }
        public long RequiredPrivileges { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        private List<Client>               _clients;
        private Dictionary<string, Client> _clientsByName;
        private Dictionary<int, Client>    _clientsById;

        public Channel() {
            this._clients       = new List<Client>();
            this._clientsByName = new Dictionary<string, Client>();
            this._clientsById   = new Dictionary<int, Client>();
        }

        public bool Join(Client client) {
            //If it's already joined, no point in trying to join again
            if (this._clients.Contains(client))
                return false;

            //If it's not there, but theres a client with its name or id, we should kick out the old client
            if (!this._clients.Contains(client) &&
                (this._clientsByName.ContainsKey(client.Username) || this._clientsById.ContainsKey(client.Id))
            ) {
                this._clientsByName.TryGetValue(client.Username, out Client foundClientByName);
                this._clientsById.TryGetValue(client.Id, out Client foundClientById);

                if (foundClientByName != null) {
                    this._clients.Remove(foundClientByName);
                    this._clientsByName.Remove(foundClientByName.Username);
                    this._clientsById.Remove(foundClientByName.Id);
                }

                if (foundClientById != null) {
                    this._clients.Remove(foundClientById);
                    this._clientsByName.Remove(foundClientById.Username);
                    this._clientsById.Remove(foundClientById.Id);
                }

                if (foundClientById == foundClientByName) {
                    foundClientById?.Notify($"You have been forcibly kicked out of the {Name} channel.");
                } else {
                    foundClientById?.Notify($"You have been forcibly kicked out of the {Name} channel.");
                    foundClientByName?.Notify($"You have been forcibly kicked out of the {Name} channel.");
                }
            }

            this._clients.Add(client);
            this._clientsByName.Add(client.Username, client);
            this._clientsById.Add(client.Id, client);

            //TODO: privileges and stuff

            client.Notify($"You have successfully joined the {Name} channel!");

            return true;
        }

        public void Leave(Client client) {
            this._clients.Remove(client);
            this._clientsById.Remove(client.Id);
            this._clientsByName.Remove(client.Username);

            client.Notify($"You have successfully left {Name}");
        }

        public void SendMessage(Client sender, Message message) {
            //TODO: privileges check

            foreach (Client client in this._clients) {
                if(client != sender)
                    if(client is ClientOsu clientOsu)
                        clientOsu.SendIrcMessage(message);
            }

            ThreadHelper.SpawnThread(() => {
                const string insertLogSql = "INSERT INTO tofu.irc_log (channel, sender_name, sender_id, message) VALUES (@channel, @sender_name, @sender_id, @message)";

                MySqlParameter[] insertLogParams = new [] {
                    new MySqlParameter("@channel", this.Name),
                    new MySqlParameter("@sender_name", sender.Username),
                    new MySqlParameter("@sender_id", sender.Id),
                    new MySqlParameter("@message", message.Content)
                };

                MySqlDatabaseHandler.MySqlNonQuery(CommonGlobal.DatabaseContext, insertLogSql, insertLogParams);
            });
        }
    }
}
