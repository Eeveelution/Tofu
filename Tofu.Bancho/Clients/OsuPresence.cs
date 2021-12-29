using Tofu.Bancho.Packets.Build282.Enums;

namespace Tofu.Bancho.Clients {
    public struct OsuPresence {
        public Status UserStatus;
        public string BeatmapChecksum;
        public string StatusText;
        public Mods   EnabledMods;
    }
}
