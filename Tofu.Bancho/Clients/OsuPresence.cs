using Tofu.Bancho.Packets.Build282.Enums;

namespace Tofu.Bancho.PacketObjects {
    public class OsuPresence {
        public Status UserStatus      = Status.Idle;
        public string BeatmapChecksum = "";
        public string StatusText      = "";
        public Mods   EnabledMods     = Mods.None;
    }
}
