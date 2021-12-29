using Tofu.Bancho.Helpers.BanchoSerializer;

namespace Tofu.Bancho.Packets.Build282 {
    public class BanchoLoginResponse : Serializable {
        [BanchoSerialize] public int UserId;

        public BanchoLoginResponse(int userId) => this.UserId = userId;

        public Packet ToPacket() => new(RequestType.BanchoLoginReply, this);

        public static implicit operator Packet(BanchoLoginResponse response) => response.ToPacket();
    }
}
