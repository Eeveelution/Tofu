using System.IO;
using System.Net.Sockets;

namespace Tofu.Bancho.Clients {
    /// <summary>
    /// Just a normal Client
    /// </summary>
    public abstract class Client {
        /// <summary>
        /// It's TCP Socket
        /// </summary>
        protected TcpClient     TcpClient;
        /// <summary>
        /// It's Network Stream
        /// </summary>
        protected NetworkStream ClientStream;
        /// <summary>
        /// Writer that writes to the Client Stream
        /// </summary>
        protected BinaryWriter  StreamWriter;

        /// <summary>
        /// The Client's information
        /// </summary>
        public ClientInformation ClientInformation;

        /// <summary>
        /// Creates a raw client
        /// </summary>
        /// <param name="bancho">Bancho</param>
        /// <param name="client">TCP Socket</param>
        public Client(TcpClient client) {
            this.TcpClient    = client;
            this.ClientStream = client.GetStream();
            this.StreamWriter = new BinaryWriter(this.ClientStream);
        }
        /// <summary>
        /// Used for Handling everything client related, this gets called by TofuWorkers
        /// </summary>
        public abstract void HandleClient();
        /// <summary>
        /// Used for Authenticating the Client
        /// </summary>
        /// <returns>Authentication success</returns>
        public virtual bool PerformAuth() { return true; }
        /// <summary>
        /// Used for Cleaning up and killing the client
        /// </summary>
        /// <param name="reason">Potential reason for killing this client</param>
        public abstract void Kill(string reason);
        /// <summary>
        /// Gets called after Registration of this client was successful
        /// </summary>
        public virtual void RegistrationComplete() {}
    }
}
