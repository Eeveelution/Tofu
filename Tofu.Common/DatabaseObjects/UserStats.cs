namespace Tofu.Common.DatabaseObjects {
    public class UserStats {
        public long StatId { get; set; }
        public long UserId { get; set; }
        public byte Mode { get; set; }
        public long RankedScore { get; set; }
        public long TotalScore { get; set; }
        public double Performance { get; set; }
        public double Level { get; set; }
        public double Accuracy { get; set; }
        public long Playcount { get; set; }
        public long CountSsh { get; set; }
        public long CountSs { get; set; }
        public long CountSh { get; set; }
        public long CountS { get; set; }
        public long CountA { get; set; }
        public long CountB { get; set; }
        public long CountC { get; set; }
        public long CountD { get; set; }
        public long Hit300 { get; set; }
        public long Hit100 { get; set; }
        public long Hit50 { get; set; }
        public long HitMiss { get; set; }
        public long HitGeki { get; set; }
        public long HitKatu { get; set; }
        public long Rank { get; set; }
    }
}
