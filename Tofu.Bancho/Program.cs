using EeveeTools.Database;
using Kettu;using Tofu.Bancho;

Logger.StartLogging();
Logger.AddLogger(new ConsoleLogger());

DatabaseContext context = new DatabaseContext("root", "root", "127.0.0.1", "tofu");

new Bancho("127.0.0.1", 13381, context).RunBancho();

Logger.StopLogging();