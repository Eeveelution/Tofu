using System.Net.Sockets;
using Kettu;
using Tofu.Bancho.Logging;

namespace Tofu.Bancho.Clients.OsuClients {
    public class ClientBuild281 : ClientOsu {
        private Bancho _bancho;

        public ClientBuild281(Bancho bancho, TcpClient client, ClientInformation information) : base(client) {
            this.ClientInformation = information;
            this._bancho           = bancho;
        }

        public override void HandleClient() {

        }

        public override void Kill() {
            this._bancho.ClientManager.RemoveClient(this);
        }

        public override void RegistrationComplete() {

        }
    }
}
