using Tofu.Bancho.Helpers.BanchoSerializer;
using Tofu.Bancho.Packets.Build282.Enums;

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
        public static implicit operator Packet(BanchoHandleOsuUpdate response) => response.ToPacket();
    }
}
