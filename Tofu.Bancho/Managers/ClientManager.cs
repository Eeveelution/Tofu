using System;
using System.Collections.Generic;
using Tofu.Bancho.Clients;
using Tofu.Bancho.Clients.OsuClients;

namespace Tofu.Bancho.Managers {
    public class ClientManager {
        internal readonly Dictionary<string, Client> ClientsByName;
        internal readonly Dictionary<int, Client>    ClientsById;

        internal readonly Dictionary<string, ClientOsu> OsuClientsByName;
        internal readonly Dictionary<int, ClientOsu>    OsuClientsById;

        internal readonly List<Client> Clients;
        internal readonly List<ClientOsu> OsuClients;

        private readonly object _clientListLock;
        /// <summary>
        /// Creates a ClientManager instance
        /// </summary>
        /// <param name="bancho">Bancho this is running under</param>
        public ClientManager() {
            this.Clients          = new List<Client>();
            this.OsuClients       = new List<ClientOsu>();
            this.ClientsByName    = new Dictionary<string, Client>();
            this.ClientsById      = new Dictionary<int, Client>();
            this.OsuClientsByName = new Dictionary<string, ClientOsu>();
            this.OsuClientsById   = new Dictionary<int, ClientOsu>();
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
                existingClient = this.GetClientById(client.Id);
                existingClient?.Kill("Duplicate Client.");

                existingClient = this.GetClientByName(client.Username);
                existingClient?.Kill("Duplicate Client.");

                //Add it to all the lists
                this.Clients.Add(client);
                this.ClientsByName.Add(client.Username, client);
                this.ClientsById.Add(client.Id, client);

                //If it's an osu! client add it to those respective lists
                if (client is ClientOsu clientOsu) {
                    this.OsuClients.Add(clientOsu);
                    this.OsuClientsByName.Add(client.Username, clientOsu);
                    this.OsuClientsById.Add(client.Id, clientOsu);
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
                this.Clients.Remove(client);
                this.ClientsByName.Remove(client.Username);
                this.ClientsById.Remove(client.Id);

                if (client is ClientOsu clientOsu) {
                    this.OsuClientsByName.Remove(client.Username);
                    this.OsuClientsById.Remove(client.Id);
                    this.OsuClients.Remove(clientOsu);
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
            this.ClientsByName.TryGetValue(name, out foundClient);

            return foundClient;
        }
        /// <summary>
        /// Gets a client by Name
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Client with that User Id</returns>
        public Client GetClientById(int id) {
            Client foundClient;
            this.ClientsById.TryGetValue(id, out foundClient);

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
                    int count = this.Clients.Count;

                    if (count == 0)
                        return null;

                    //Work out what range of clients each worker should process
                    int range = (int) Math.Ceiling((float) count / Global.Bancho.GetTofuWorkerCount());
                    //Determine it's start
                    int start = range * worker.Id;

                    //Get its index using the start and count, and what last client the worker processed
                    int index = Math.Min(count - 1, start + worker.LastProcessedIndex);

                    if (index < start)
                        return null;

                    Client client = this.Clients[index];

                    //Increase its last processed client, so that next time this worker comes around, it will pick the next client
                    worker.LastProcessedIndex = (worker.LastProcessedIndex + 1) % range;

                    return client;
                }
            }
            catch {
                return null;
            }
        }
        /// <summary>
        /// Broadcasts a osu! Packet to everyone
        /// </summary>
        /// <param name="packet">Packet to Broadcast</param>
        public void BroadcastPacketOsu(Action<ClientOsu> packet) {
            for (int i = 0; i < this.OsuClients.Count; i++) {
                lock (this._clientListLock) {
                    ClientOsu clientOsu = this.OsuClients[i];

                    packet.Invoke(clientOsu);
                }
            }
        }
        /// <summary>
        /// Broadcasts a Packet to everyone but themselves
        /// </summary>
        /// <param name="packet">Packet to Broadcast</param>
        /// <param name="self">Self</param>
        public void BroadcastPacketOsuExceptSelf(Action<ClientOsu> packet, ClientOsu self) {
            for (int i = 0; i < this.OsuClients.Count; i++) {
                lock (this._clientListLock) {
                    ClientOsu clientOsu = this.OsuClients[i];

                    if(clientOsu == self)
                        continue;

                    packet.Invoke(clientOsu);
                }
            }
        }
    }
}
