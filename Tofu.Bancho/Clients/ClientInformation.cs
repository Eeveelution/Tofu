using Tofu.Bancho.DatabaseObjects;
using Tofu.Bancho.PacketObjects;
using Tofu.Bancho.PacketObjects.Enums;

namespace Tofu.Bancho.Clients {
    public struct ClientInformation {
        /// <summary>
        /// Everything known about the User
        /// </summary>
        public User User;
        /// <summary>
        /// User Id of the CLient
        /// </summary>
        public int Id => (int) User.Id;
        /// <summary>
        /// Username of the Client
        /// </summary>
        public string Username => User.Username;
        /// <summary>
        /// Timezone of the Client
        /// </summary>
        public int Timezone;
        /// <summary>
        /// Whether the Login succeeded prior to Upgrading
        /// </summary>
        public LoginResult PendingLoginResult;
        /// <summary>
        /// osu! Presence Information
        /// </summary>
        public OsuPresence Presence;
        /// <summary>
        /// Current Playmode
        /// </summary>
        public byte CurrentPlayMode;

        public Stats GetStats(byte mode) {
            UserStats stats = mode switch {
                0 => this.User.OsuStats,
                1 => this.User.TaikoStats,
                2 => this.User.CatchStats,
                3 => this.User.ManiaStats,
            };

            return new Stats {
                UserId      = (int) this.User.Id,
                Username    = this.User.Username,
                RankedScore = stats.RankedScore,
                Accuracy    = stats.Accuracy,
                Playcount   = (int) stats.Playcount,
                TotalScore  = stats.TotalScore,
                Presence = new OsuPresence {
                    BeatmapChecksum = this.Presence.BeatmapChecksum,
                    UserStatus      = this.Presence.UserStatus,
                    StatusText      = this.Presence.StatusText,
                    EnabledMods     = this.Presence.EnabledMods,
                },
                Timezone = (byte) this.Timezone,
                Location = this.User.Location
            };
        }
    }
}
