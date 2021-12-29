using System.IO;
using EeveeTools.Helpers;
using Tofu.Bancho.Helpers.BanchoSerializer;

using RequestType282 = Tofu.Bancho.Packets.Build282.RequestType;

namespace Tofu.Bancho.Packets {
    public class Packet {
        public short  PacketId;
        public bool   Compressed;
        public int    PacketSize;
        public byte[] Payload;

        public Packet(short packetId, Serializable payload) {
            this.PacketId   = packetId;
            this.Payload    = payload.ToBytes();
            this.PacketSize = this.Payload.Length;
            this.Compressed = this.PacketSize >= 128;
        }

        public Packet(RequestType282 packetId, Serializable payload) : this((short) packetId, payload) {}

        public void Send(Stream stream, bool sendCompressionByte = true) {
            using BanchoWriter writer = new(stream);

            writer.Write(this.PacketId);

            if(sendCompressionByte)
                writer.Write(this.Compressed);

            writer.Write(this.PacketSize);
            writer.Write(this.Payload);
        }
    }
}
