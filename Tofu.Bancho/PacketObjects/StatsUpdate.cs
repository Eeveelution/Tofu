namespace Tofu.Bancho.PacketObjects {
    public class StatsUpdate {
        public int         UserId;
        public string      Username;
        public long        RankedScore;
        public double      Accuracy;
        public int         Playcount;
        public long        TotalScore;
        public int         Rank;
        public OsuPresence Presence;
        public byte        Timezone;
        public string      Location;
    }
}
