using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Tofu.Bancho {
    public class Bancho {
        private TcpListener      _banchoListener;
        private List<TofuWorker> _tofuWorkers;

        public ClientManager ClientManager;

        public Bancho(string location, int port) {
            this._banchoListener = new TcpListener(IPAddress.Parse(location), port);
            this._tofuWorkers    = new List<TofuWorker>();

            this.ClientManager = new ClientManager(this);
        }

        public int GetTofuWorkerCount() => this._tofuWorkers.Count;

        public async Task Run() {
            this._banchoListener.Start();

            while (true) {
                TcpClient newClient = await this._banchoListener.AcceptTcpClientAsync();
            }
        }
    }
}
