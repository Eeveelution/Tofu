using System.Threading.Tasks;

namespace Tofu.Bancho {
    public abstract class Client {
        public abstract Task HandleClient();
    }
}
