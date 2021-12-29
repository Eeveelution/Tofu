using System;
using System.Threading;
using Kettu;
using Tofu.Bancho.Clients;
using Tofu.Bancho.Logging;

namespace Tofu.Bancho {
    public class TofuWorker {
        private Bancho _bancho;

        public int Id { get; }

        public int      LastProcessedIndex;
        public DateTime LastClientHandleRequest;

        private Thread _workerThread;
        private bool   _continueWork = true;

        public TofuWorker(Bancho bancho, int id) {
            this._bancho = bancho;

            this.Id                      = id;
            this.LastClientHandleRequest = DateTime.MinValue;

            this._workerThread = new Thread(this.Work);
        }

        public void Start() {
            this._workerThread.Start();
        }

        public void Stop() {
            this._continueWork = false;
            this._workerThread.Join();
        }

        private void Work() {
            while (this._continueWork) {
                Client client = this._bancho.ClientManager.GetProcessableClient(this);

                if (client != null) {
                    client.HandleClient();
                }

                Thread.Sleep(20);
            }
        }
    }
}
