using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using Tofu.Bancho.PacketObjects;
using Tofu.Bancho.PacketObjects.Enums;
using Tofu.Bancho.Packets;

namespace Tofu.Bancho.Clients.OsuClients {
    public abstract class ClientOsu : Client {
        /// <summary>
        /// The Outgoing Packet Queue
        /// </summary>
        protected ConcurrentQueue<Packet> PacketQueue;

        /// <summary>
        /// The Last Time we pinged the Client
        /// </summary>
        protected DateTime LastPingTime;
        /// <summary>
        /// The Last Time the client got back to us
        /// </summary>
        protected DateTime LastPongTime;

        /// <summary>
        /// Ping timeout, how often pings should be sent
        /// </summary>
        protected readonly TimeSpan PingTimeout = new TimeSpan(0, 0, 0, 10);
        /// <summary>
        /// Pong timeout, after how many seconds should we disconnect the client if we don't get anything
        /// </summary>
        protected readonly TimeSpan PongTimeout = new TimeSpan(0, 0, 0, 20);

        /// <summary>
        /// Creates a ClientOsu
        /// </summary>
        /// <param name="bancho">Bancho it's connected to</param>
        /// <param name="client">TcpClient</param>
        public ClientOsu(TcpClient client) : base(client) {
            this.PacketQueue  = new ConcurrentQueue<Packet>();
            this.LastPingTime = DateTime.MinValue;
            this.LastPongTime = DateTime.MinValue;
        }

        protected void QueuePacket(Packet packet) => this.PacketQueue.Enqueue(packet);

        #region Packet Functions

        public virtual void LoginResponse(LoginResult result) { this.LoginResponse((int) result); }
        public abstract void LoginResponse(int userId);
        public abstract void Ping();
        public abstract void HandleOsuUpdate(Stats update);

        #endregion

    }
}