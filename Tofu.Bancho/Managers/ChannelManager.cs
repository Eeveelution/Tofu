using System;
using System.Collections.Generic;
using EeveeTools.Database;
using Kettu;
using MySqlConnector;
using Tofu.Bancho.Clients;
using Tofu.Bancho.Logging;
using Tofu.Bancho.PacketObjects;
using Tofu.Common;
using Tofu.Common.Helpers;

namespace Tofu.Bancho.Managers {
    public class ChannelManager {
        private List<Channel>               _channels;
        private Dictionary<string, Channel> _channelsByName;

        public ChannelManager() {
            this._channels       = new List<Channel>();
            this._channelsByName = new Dictionary<string, Channel>();
        }

        public void ReadFromDatabase(bool clearAll = false) {
            if (clearAll) {
                this._channels.Clear();
                this._channelsByName.Clear();
            }

            const string channelSql = "SELECT * FROM tofu.channels";

            var results = MySqlDatabaseHandler.MySqlQuery(CommonGlobal.DatabaseContext, channelSql);

            foreach (IReadOnlyDictionary<string, object> result in results) {
                Channel channel = new Channel();
                channel.MapDatabaseResults(result);

                Logger.Log($"Added Channel {channel.Name}", LoggerLevelInfo.Instance);

                this._channels.Add(channel);
                this._channelsByName.Add(channel.Name, channel);
            }
        }

        public void AddChannel(string name, string topic = "", long requiredPrivileges = 0, bool temporary = true) {
            Channel channel = new Channel {
                Name               = name,
                Topic              = topic,
                CreatedAt          = DateTime.Now,
                UpdatedAt          = DateTime.Now,
                RequiredPrivileges = requiredPrivileges,
            };

            this._channels.Add(channel);
            this._channelsByName.Add(channel.Name, channel);

            if (!temporary) {
                const string addChannelSql = "INSERT INTO tofu.channels (name, topic, required_privileges) VALUES (@name, @topic, @privileges)";

                MySqlParameter[] addChannelParams = new[] {
                    new MySqlParameter("@name", name),
                    new MySqlParameter("@topic", topic),
                    new MySqlParameter("@privileges", requiredPrivileges)
                };

                MySqlDatabaseHandler.MySqlNonQuery(CommonGlobal.DatabaseContext, addChannelSql, addChannelParams);
            }
        }

        public void RemoveChannel(string name, bool temporary) {
            this._channels.RemoveAll(channel => channel.Name == name);
            this._channelsByName.Remove(name);

            if (!temporary) {
                const string channelRemoveSql = "DELETE FROM tofu.channels WHERE channels.name = @name";

                MySqlParameter[] channelRemoveParams = new[] {
                    new MySqlParameter("@name", name)
                };

                MySqlDatabaseHandler.MySqlNonQuery(CommonGlobal.DatabaseContext, channelRemoveSql, channelRemoveParams);
            }
        }

        public bool SendMessage(Client sender, string channel, Message message) {
            this._channelsByName.TryGetValue(channel, out Channel foundChannel);

            if (foundChannel == null)
                return false;

            foundChannel.SendMessage(sender, message);

            return true;
        }

        public Channel GetChannelByName(string name) {
            this._channelsByName.TryGetValue(name, out Channel foundChannel);

            return foundChannel;
        }
    }
}
