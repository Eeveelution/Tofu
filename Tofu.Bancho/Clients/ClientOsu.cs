using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using Tofu.Bancho.Packets;

namespace Tofu.Bancho.Clients {
    public abstract class ClientOsu : Client {
        protected ConcurrentQueue<Packet> PacketQueue;

        protected DateTime                LastPingTime;
        protected DateTime                LastPongTime;

        protected readonly TimeSpan PingTimeout = new TimeSpan(0, 0, 0, 10);
        protected readonly TimeSpan PongTimeout = new TimeSpan(0, 0, 0, 20);

        public ClientOsu(Bancho bancho, TcpClient client) : base(bancho, client) {
            this.PacketQueue  = new ConcurrentQueue<Packet>();
            this.LastPingTime = DateTime.MinValue;
            this.LastPongTime = DateTime.MinValue;
        }
    }
}
