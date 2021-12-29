using Kettu;

namespace Tofu.Bancho.Logging {
    public class LoggerLevelInfo : LoggerLevel {
        public override string Name => "Info";
        public static LoggerLevelInfo Instance = new();
    }

    public class LoggerLevelError : LoggerLevel {
        public override string Name => "Error";
        public static LoggerLevelInfo Instance = new();
    }

    public class LoggerLevelWarning : LoggerLevel {
        public override string Name => "Warning";
        public static LoggerLevelInfo Instance = new();
    }

    public class LoggerLevelWorker : LoggerLevel {
        public override string Name => "Workers";
        public static LoggerLevelInfo Instance = new();
    }
}
