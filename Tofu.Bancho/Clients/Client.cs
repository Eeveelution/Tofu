using System.IO;
using System.Net.Sockets;
using Tofu.Bancho.DatabaseObjects;
using Tofu.Bancho.PacketObjects;
using Tofu.Bancho.PacketObjects.Enums;

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
        /// Everything known about the User
        /// </summary>
        public User User;
        /// <summary>
        /// User Id of the CLient
        /// </summary>
        public int Id => (int) User.Id;
        /// <summary>
        /// Username of the Client
        /// </summary>
        public string Username => User.Username;

        /// <summary>
        /// Creates a raw client
        /// </summary>
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
