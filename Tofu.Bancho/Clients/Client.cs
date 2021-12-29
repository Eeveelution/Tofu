using System.Net.Sockets;

namespace Tofu.Bancho.Clients {
    public abstract class Client {
        protected TcpClient         TcpClient;
        protected NetworkStream     ClientStream;

        public ClientInformation ClientInformation;



        public abstract void HandleClient();
        public virtual bool Authenticate() { return true; }
        public abstract void Kill();
        public virtual void RegistrationComplete() {}
    }
}
