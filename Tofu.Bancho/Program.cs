using Kettu;using Tofu.Bancho;

Logger.StartLogging();
Logger.AddLogger(new ConsoleLogger());

new Bancho("127.0.0.1", 13381).RunBancho();

Logger.StopLogging();