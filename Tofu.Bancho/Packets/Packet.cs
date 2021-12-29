using System.IO;
using EeveeTools.Helpers;
using Tofu.Bancho.Helpers.BanchoSerializer;

using RequestType282 = Tofu.Bancho.Packets.Build282.Enums.RequestType;

namespace Tofu.Bancho.Packets {
    /// <summary>
    /// A Bancho Packet
    /// </summary>
    public class Packet {
        /// <summary>
        /// ID Of the Packet
        /// </summary>
        public short  PacketId;
        /// <summary>
        /// Whether or not it's compressed
        /// </summary>
        public bool   Compressed;
        /// <summary>
        /// The size of the Packet
        /// </summary>
        public int    PacketSize;
        /// <summary>
        /// And the Payload or data of the Packet
        /// </summary>
        public byte[] Payload;

        /// <summary>
        /// Creates a Packet
        /// </summary>
        /// <param name="packetId">Packet ID</param>
        /// <param name="payload">Packet Data</param>
        public Packet(short packetId, Serializable payload) {
            this.PacketId   = packetId;
            this.Payload    = payload.ToBytes();
            this.PacketSize = this.Payload.Length;
            this.Compressed = this.PacketSize >= 128;
        }

        /// <summary>
        /// Creates a Packet using the b282 RequestType enum
        /// </summary>
        /// <param name="packetId">Packet ID</param>
        /// <param name="payload">Packet Data</param>
        public Packet(RequestType282 packetId, Serializable payload) : this((short) packetId, payload) {}

        /// <summary>
        /// Sends/Writes this packet to a stream
        /// </summary>
        /// <param name="stream">Stream to send to</param>
        /// <param name="sendCompressionByte">Because b282 is dumb and forces compression by default, this exists as it doesnt even have it in the header</param>
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
