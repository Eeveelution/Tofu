using System;
using System.Threading;
using Kettu;
using Tofu.Bancho.Clients;
using Tofu.Bancho.Logging;

namespace Tofu.Bancho {
    public class TofuWorker {
        /// <summary>
        /// Bancho it's working for
        /// </summary>
        private Bancho _bancho;

        /// <summary>
        /// It's ID
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Last Client index it processed
        /// </summary>
        public int      LastProcessedIndex;
        /// <summary>
        /// Last time the worker asked for a Client to handle
        /// </summary>
        public DateTime LastClientHandleRequest;

        /// <summary>
        /// It's Thread
        /// </summary>
        private Thread _workerThread;
        /// <summary>
        /// Whether it should continue working
        /// </summary>
        private bool   _continueWork = true;

        /// <summary>
        /// Creates a TofuWorker
        /// </summary>
        /// <param name="bancho">Bancho it's working for</param>
        /// <param name="id">It's ID</param>
        public TofuWorker(Bancho bancho, int id) {
            this._bancho = bancho;

            this.Id                      = id;
            this.LastClientHandleRequest = DateTime.MinValue;

            this._workerThread = new Thread(this.Work);
        }

        /// <summary>
        /// Starts the Worker
        /// </summary>
        public void Start() {
            this._workerThread.Start();
        }
        /// <summary>
        /// Stops the Worker
        /// </summary>
        public void Stop() {
            this._continueWork = false;
            this._workerThread.Join();
        }

        /// <summary>
        /// Work Thread
        /// </summary>
        private void Work() {
            while (this._continueWork) {
                //Get Client to process
                Client client = this._bancho.ClientManager.GetProcessableClient(this);
                this.LastClientHandleRequest = DateTime.Now;

                //Handle it
                client?.HandleClient();

                //Sleep
                //TODO: use peppys crazy algorithm to make sleeps as efficient as possible, to not sleep too long or too little
                Thread.Sleep(20);
            }
        }
    }
}
