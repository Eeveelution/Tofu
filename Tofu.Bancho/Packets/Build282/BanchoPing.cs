using Tofu.Bancho.Helpers.BanchoSerializer;

namespace Tofu.Bancho.Packets.Build282 {
    public class BanchoPing : Serializable {
        public Packet ToPacket() => new(RequestType.BanchoPing, this);

        public static implicit operator Packet(BanchoPing response) => response.ToPacket();
    }
}
