using Tofu.Bancho.Helpers.BanchoSerializer;
using Tofu.Bancho.Packets.Build282.Enums;

namespace Tofu.Bancho.Packets.Common {
    /// <summary>
    /// Just a normal ping packet, used to ping the client to make sure it's still connected
    /// </summary>
    public class BanchoPing : Serializable {
        public Packet ToPacket() => new(RequestType.BanchoPing, this);
        public static implicit operator Packet(BanchoPing response) => response.ToPacket();
    }
}
