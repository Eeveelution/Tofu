using System.Net.Sockets;
using System.Threading.Tasks;

namespace Tofu.Bancho {
    public class UnauthenticatedClient : ClientOsu {
        public UnauthenticatedClient(TcpClient client) {

        }

        public override async Task HandleClient() {

        }
    }
}
