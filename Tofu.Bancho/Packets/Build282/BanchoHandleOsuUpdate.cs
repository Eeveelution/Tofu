using Tofu.Bancho.Clients.OsuClients;
using Tofu.Bancho.Helpers.BanchoSerializer;
using Tofu.Bancho.Packets.Build282.Enums;
using Tofu.Bancho.Packets.Common.Enums;
using Tofu.Common.DatabaseObjects;

namespace Tofu.Bancho.Packets.Build282 {
    public class BanchoHandleOsuUpdate : Serializable {
        [BanchoSerialize] public int UserId;
        [BanchoSerialize] public string Username;
        [BanchoSerialize] public long RankedScore;
        [BanchoSerialize] public double Accuracy;
        [BanchoSerialize] public int Playcount;
        [BanchoSerialize] public long TotalScore;
        [BanchoSerialize] public int Rank;
        [BanchoSerialize] public string AvatarFilename;
        [BanchoSerialize] public Status UserStatus;
        [BanchoSerialize] public string StatusText;
        [BanchoSerialize] public string BeatmapChecksum;
        [BanchoSerialize] public Mods Mods;
        [BanchoSerialize] public byte Timezone;
        [BanchoSerialize] public string Location;

        public Packet ToPacket() => new(RequestType.BanchoHandleOsuUpdate, this);
        public static BanchoHandleOsuUpdate Create(ClientOsu clientOsu) {
            UserStats stats = clientOsu.CurrentPlayMode switch {
                Playmode.Osu   => clientOsu.User.OsuStats,
                Playmode.Taiko => clientOsu.User.TaikoStats,
                Playmode.Catch => clientOsu.User.CatchStats,
                Playmode.Mania => clientOsu.User.ManiaStats,
                _              => clientOsu.User.OsuStats
            };

            return new BanchoHandleOsuUpdate {
                UserId          = clientOsu.Id,
                Username        = clientOsu.Username,
                RankedScore     = stats.RankedScore,
                TotalScore      = stats.TotalScore,
                Accuracy        = stats.Accuracy,
                Playcount       = (int) stats.Playcount,
                Rank            = (int) stats.Rank,
                AvatarFilename  = clientOsu.Username,
                StatusText      = clientOsu.Presence.StatusText,
                BeatmapChecksum = clientOsu.Presence.BeatmapChecksum,
                Mods            = clientOsu.Presence.EnabledMods,
                UserStatus      = clientOsu.Presence.UserStatus,
                Timezone        = clientOsu.ClientData.Timezone,
                Location        = clientOsu.User.Location
            };
        }
        public static implicit operator Packet(BanchoHandleOsuUpdate response) => response.ToPacket();
    }
}
