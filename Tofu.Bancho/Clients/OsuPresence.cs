using Tofu.Bancho.Packets.Build282.Enums;
using Tofu.Bancho.Packets.Common.Enums;

namespace Tofu.Bancho.Clients {
    public class OsuPresence {
        public Status UserStatus      = Status.Idle;
        public string BeatmapChecksum = "";
        public string StatusText      = "";
        public Mods   EnabledMods     = Mods.None;
    }
}
