using EeveeTools.Database;
using Kettu;using Tofu.Bancho;
using Tofu.Common;

Logger.StartLogging();
Logger.AddLogger(new ConsoleLogger());

CommonGlobal.DatabaseContext = new DatabaseContext("root", "root", "127.0.0.1", "tofu");

Global.Bancho = new Bancho("127.0.0.1", 13381);
Global.Bancho.RunBancho();

Logger.StopLogging();