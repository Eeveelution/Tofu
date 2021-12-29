using Tofu.Bancho.Helpers.BanchoSerializer;

namespace Tofu.Bancho.Packets.Build282 {
    /// <summary>
    /// Just a normal ping packet, used to ping the client to make sure it's still connected
    /// </summary>
    public class BanchoPing : Serializable {
        public Packet ToPacket() => new(RequestType.BanchoPing, this);
        public static implicit operator Packet(BanchoPing response) => response.ToPacket();
    }
}