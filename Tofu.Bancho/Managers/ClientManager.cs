using System;
using System.Collections.Generic;

namespace Tofu.Bancho {
    public class ClientManager {
        private Bancho _bancho;

        private Dictionary<string, Client> _clientsByName;
        private Dictionary<long, Client>   _clientsById;
        private List<Client>               _clients;

        public ClientManager(Bancho bancho) {
            this._bancho        = bancho;
            this._clients       = new List<Client>();
            this._clientsByName = new Dictionary<string, Client>();
            this._clientsById   = new Dictionary<long, Client>();
        }

        public Client GetClientByName(string name) {
            Client foundClient;
            this._clientsByName.TryGetValue(name, out foundClient);

            return foundClient;
        }

        public Client GetClientById(string name) {
            Client foundClient;
            this._clientsByName.TryGetValue(name, out foundClient);

            return foundClient;
        }

        public Client GetProcessableClient(TofuWorker worker) {
            try {
                int count = this._clients.Count;

                if (count == 0)
                    return null;

                int range = (int) Math.Ceiling((float) count / this._bancho.GetTofuWorkerCount());
                int start = range * worker.Id;

                int index = Math.Min(count - 1, start + worker.LastProcessedIndex);

                if (index < start)
                    return null;

                Client client = this._clients[index];

                worker.LastProcessedIndex = (worker.LastProcessedIndex + 1) % range;

                return client;
            }
            catch (Exception e) {
                return null;
            }
        }
    }
}
