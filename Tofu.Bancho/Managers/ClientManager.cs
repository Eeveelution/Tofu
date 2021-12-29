using System;
using System.Collections.Generic;
using Tofu.Bancho.Clients;

namespace Tofu.Bancho.Managers {
    public class ClientManager {
        private readonly Bancho _bancho;

        private readonly Dictionary<string, Client> _clientsByName;
        private readonly Dictionary<int, Client>    _clientsById;

        private readonly Dictionary<string, ClientOsu> _osuClientsByName;
        private readonly Dictionary<int, ClientOsu>    _osuClientsById;

        private readonly List<Client>               _clients;

        private readonly object _clientListLock;

        public ClientManager(Bancho bancho) {
            this._bancho           = bancho;
            this._clients          = new List<Client>();
            this._clientsByName    = new Dictionary<string, Client>();
            this._clientsById      = new Dictionary<int, Client>();
            this._osuClientsByName = new Dictionary<string, ClientOsu>();
            this._osuClientsById   = new Dictionary<int, ClientOsu>();
            this._clientListLock   = new object();
        }

        public bool RegisterClient(Client client) {
            lock (this._clientListLock) {
                Client existingClient;

                existingClient = this.GetClientById(client.ClientInformation.Id);
                existingClient?.Kill("Duplicate Client.");

                existingClient = this.GetClientByName(client.ClientInformation.Username);
                existingClient?.Kill("Duplicate Client.");

                this._clients.Add(client);
                this._clientsByName.Add(client.ClientInformation.Username, client);
                this._clientsById.Add(client.ClientInformation.Id, client);

                if (client is ClientOsu clientOsu) {
                    this._osuClientsByName.Add(client.ClientInformation.Username, clientOsu);
                    this._osuClientsById.Add(client.ClientInformation.Id, clientOsu);
                }

                client.RegistrationComplete();

                return true;
            }
        }

        public void RemoveClient(Client client) {
            lock(this._clientListLock){
                this._clients.Remove(client);
                this._clientsByName.Remove(client.ClientInformation.Username);
                this._clientsById.Remove(client.ClientInformation.Id);

                if (client is ClientOsu) {
                    this._osuClientsByName.Remove(client.ClientInformation.Username);
                    this._osuClientsById.Remove(client.ClientInformation.Id);
                }
            }
        }

        public Client GetClientByName(string name) {
            Client foundClient;
            this._clientsByName.TryGetValue(name, out foundClient);

            return foundClient;
        }

        public Client GetClientById(int id) {
            Client foundClient;
            this._clientsById.TryGetValue(id, out foundClient);

            return foundClient;
        }

        public Client GetProcessableClient(TofuWorker worker) {
            try {
                lock (this._clientListLock) {
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
            }
            catch {
                return null;
            }
        }
    }
}
