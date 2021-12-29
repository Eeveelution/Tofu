using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Kettu;
using Tofu.Bancho.Clients;
using Tofu.Bancho.Helpers;
using Tofu.Bancho.Logging;
using Tofu.Bancho.Managers;

namespace Tofu.Bancho {
    public class Bancho {
        private TcpListener      _banchoListener;

        private List<TofuWorker> _tofuWorkers;
        public  ClientManager    ClientManager;

        private const int WorkerCount = 1;

        public Bancho(string location, int port) {
            this._banchoListener = new TcpListener(IPAddress.Parse(location), port);
            this._tofuWorkers    = new List<TofuWorker>();

            this.ClientManager = new ClientManager(this);

            for (int i = 0; i != WorkerCount; i++) {
                this.AddWorker();
            }
        }

        public int GetTofuWorkerCount() => this._tofuWorkers.Count;

        public void AddWorker() {
            TofuWorker worker = new TofuWorker(this, this._tofuWorkers.Count);
            worker.Start();

            this._tofuWorkers.Add(worker);

            Logger.Log($"Created new worker; id: {worker.Id}", LoggerLevelWorker.Instance);
        }

        public void RemoveWorker() {
            TofuWorker worker = this._tofuWorkers[^1];
            worker.Stop();

            this._tofuWorkers.Remove(worker);

            Logger.Log($"Removed worker; id: {worker.Id}", LoggerLevelWorker.Instance);
        }

        public void RunBancho() {
            Logger.Log("Bancho Server started!", LoggerLevelInfo.Instance);

            this._banchoListener.Start();

            while (true) {
                TcpClient newClient = this._banchoListener.AcceptTcpClient();

                Logger.Log("Recieved TcpClient on Bancho", LoggerLevelInfo.Instance);

                ThreadHelper.SpawnThread(() => {
                    UnauthenticatedClientOsu unauthenticatedClientOsu = new UnauthenticatedClientOsu(this, newClient);

                    if (unauthenticatedClientOsu.Authenticate()) {
                        ClientOsu clientOsu = unauthenticatedClientOsu.ToClientOsu();

                        this.ClientManager.RegisterClient(clientOsu);
                    }
                });
            }
        }
    }
}
