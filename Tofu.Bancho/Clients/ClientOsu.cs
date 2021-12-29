using System.Net.Sockets;

namespace Tofu.Bancho.Clients {
    public abstract class ClientOsu : Client {
        public ClientOsu(TcpClient client) {
            this.TcpClient    = client;
            this.ClientStream = client.GetStream();
        }
    }
}
