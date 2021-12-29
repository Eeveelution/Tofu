using System.IO;
using System.Net.Sockets;

namespace Tofu.Bancho.Clients {
    public abstract class Client {
        protected Bancho        Bancho;
        protected TcpClient     TcpClient;
        protected NetworkStream ClientStream;
        protected BinaryWriter  StreamWriter;

        public ClientInformation ClientInformation;

        public Client(Bancho bancho, TcpClient client) {
            this.TcpClient    = client;
            this.ClientStream = client.GetStream();
            this.StreamWriter = new BinaryWriter(this.ClientStream);
            this.Bancho       = bancho;
        }

        public abstract void HandleClient();
        public virtual bool Authenticate() { return true; }
        public abstract void Kill(string reason);
        public virtual void RegistrationComplete() {}
    }
}
