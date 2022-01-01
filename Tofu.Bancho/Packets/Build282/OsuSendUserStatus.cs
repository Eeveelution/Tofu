using System.IO;
using Tofu.Bancho.Helpers.BanchoSerializer;
using Tofu.Bancho.Packets.Build282.Enums;
using Tofu.Bancho.Packets.Common.Enums;

namespace Tofu.Bancho.Packets.Build282 {
    public sealed class OsuSendUserStatus : Serializable {
        [BanchoSerialize] public Status Status;
        [BanchoSerialize] public string StatusText;
        [BanchoSerialize] public string BeatmapChecksum;
        [BanchoSerialize] public Mods   CurrentMods;

        public OsuSendUserStatus(Stream stream) => this.ReadFromStream(stream);
    }
}
