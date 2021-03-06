using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Kettu;
using Tofu.Bancho.Clients.OsuClients;
using Tofu.Bancho.Helpers;
using Tofu.Bancho.Logging;
using Tofu.Bancho.Managers;

namespace Tofu.Bancho {
    /// <summary>
    /// A Bancho Server
    /// </summary>
    public class Bancho {
        /// <summary>
        /// The TCP Server listening for osu! Clients
        /// </summary>
        private TcpListener      _banchoListener;

        /// <summary>
        /// The Client Workers
        /// </summary>
        private List<TofuWorker> _tofuWorkers;
        /// <summary>
        /// The Client Manager, handles adding and removing clients, aswell as giving work to Workers
        /// </summary>
        public ClientManager ClientManager;
        /// <summary>
        /// The Channel Manager, handles most of Chat, and adding/removing and everything Channel related
        /// </summary>
        public ChannelManager ChannelManager;

        /// <summary>
        /// How many workers to spawn
        /// </summary>
        private const int WORKER_COUNT = 1;

        /// <summary>
        /// Creates a Bancho Server
        /// </summary>
        /// <param name="location">Where to start the Server</param>
        /// <param name="port">On what port (usually 13381)</param>
        public Bancho(string location, int port) {
            this._banchoListener = new TcpListener(IPAddress.Parse(location), port);
            this._tofuWorkers    = new List<TofuWorker>();

            this.ClientManager  = new ClientManager();
            this.ChannelManager = new ChannelManager();

            //Initialize Workers
            for (int i = 0; i != WORKER_COUNT; i++) {
                this.AddWorker();
            }

            //Initialize Channels
            this.ChannelManager.ReadFromDatabase();
        }

        /// <summary>
        /// Gets the amount of currently existing workers
        /// </summary>
        /// <returns></returns>
        public int GetTofuWorkerCount() => this._tofuWorkers.Count;

        /// <summary>
        /// Creates and adds a worker
        /// </summary>
        public void AddWorker() {
            TofuWorker worker = new TofuWorker(this._tofuWorkers.Count);
            worker.Start();

            this._tofuWorkers.Add(worker);

            Logger.Log($"Created new worker; id: {worker.Id}", LoggerLevelWorker.Instance);
        }

        /// <summary>
        /// Removes a worker
        /// </summary>
        public void RemoveWorker() {
            TofuWorker worker = this._tofuWorkers[^1];
            worker.Stop();

            this._tofuWorkers.Remove(worker);

            Logger.Log($"Removed worker; id: {worker.Id}", LoggerLevelWorker.Instance);
        }

        /// <summary>
        /// Starts the Bancho,
        /// <remarks>This blocks the Thread this is called under, if you have other stuff happening you might want to put this on another Thread</remarks>
        /// </summary>
        public void RunBancho() {
            Logger.Log("Bancho Server started!", LoggerLevelInfo.Instance);

            this._banchoListener.Start();

            while (true) {
                TcpClient newClient = this._banchoListener.AcceptTcpClient();

                Logger.Log("Recieved TcpClient on Bancho", LoggerLevelInfo.Instance);

                ThreadHelper.SpawnThread(() => {
                    //Creates a Unauthenticated client,
                    //This is because we don't currently know what sort of osu! client it is,
                    //it could be b281, b394a, whatever, since we cant tell until we get the login, this handles just the login information
                    //and then later we can upgrade the connection
                    UnknownClientOsu unknownClientOsu = new UnknownClientOsu(newClient);

                    //Authenticate
                    if(unknownClientOsu.PerformAuth()) {
                        //Upgrade the Connection
                        ClientOsu clientOsu = unknownClientOsu.ToClientOsu();

                        this.ClientManager.RegisterClient(clientOsu);
                    }
                });
            }
        }
    }
}
