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
        /// <summary>
        /// Creates a ClientManager instance
        /// </summary>
        /// <param name="bancho">Bancho this is running under</param>
        public ClientManager(Bancho bancho) {
            this._bancho           = bancho;
            this._clients          = new List<Client>();
            this._clientsByName    = new Dictionary<string, Client>();
            this._clientsById      = new Dictionary<int, Client>();
            this._osuClientsByName = new Dictionary<string, ClientOsu>();
            this._osuClientsById   = new Dictionary<int, ClientOsu>();
            this._clientListLock   = new object();
        }
        /// <summary>
        /// Registers a client
        /// </summary>
        /// <param name="client">Client to register</param>
        /// <returns>Success of Registration</returns>
        public bool RegisterClient(Client client) {
            lock (this._clientListLock) {
                Client existingClient;

                //Check for duplicate clients
                existingClient = this.GetClientById(client.ClientInformation.Id);
                existingClient?.Kill("Duplicate Client.");

                existingClient = this.GetClientByName(client.ClientInformation.Username);
                existingClient?.Kill("Duplicate Client.");

                //Add it to all the lists
                this._clients.Add(client);
                this._clientsByName.Add(client.ClientInformation.Username, client);
                this._clientsById.Add(client.ClientInformation.Id, client);

                //If it's an osu! client add it to those respective lists
                if (client is ClientOsu clientOsu) {
                    this._osuClientsByName.Add(client.ClientInformation.Username, clientOsu);
                    this._osuClientsById.Add(client.ClientInformation.Id, clientOsu);
                }

                //Make client handle a complete registration
                //TODO: incase there will be a case where this fails, add a RegistrationFailed handler in Client
                client.RegistrationComplete();

                return true;
            }
        }
        /// <summary>
        /// Removes the Client
        /// </summary>
        /// <param name="client">Client to remove</param>
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
        /// <summary>
        /// Gets a client by Name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Client with that Name</returns>
        public Client GetClientByName(string name) {
            Client foundClient;
            this._clientsByName.TryGetValue(name, out foundClient);

            return foundClient;
        }
        /// <summary>
        /// Gets a client by Name
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Client with that User Id</returns>
        public Client GetClientById(int id) {
            Client foundClient;
            this._clientsById.TryGetValue(id, out foundClient);

            return foundClient;
        }
        /// <summary>
        /// This mostly exists for <see cref="TofuWorker"/> so they can get a Client to process
        /// </summary>
        /// <param name="worker">TofuWorker</param>
        /// <returns>A client to process</returns>
        public Client GetProcessableClient(TofuWorker worker) {
            try {
                lock (this._clientListLock) {
                    int count = this._clients.Count;

                    if (count == 0)
                        return null;

                    //Work out what range of clients each worker should process
                    int range = (int) Math.Ceiling((float) count / this._bancho.GetTofuWorkerCount());
                    //Determine it's start
                    int start = range * worker.Id;

                    //Get its index using the start and count, and what last client the worker processed
                    int index = Math.Min(count - 1, start + worker.LastProcessedIndex);

                    if (index < start)
                        return null;

                    Client client = this._clients[index];

                    //Increase its last processed client, so that next time this worker comes around, it will pick the next client
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
