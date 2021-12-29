using Tofu.Bancho.Helpers.BanchoSerializer;

namespace Tofu.Bancho.Packets.Build282 {
    /// <summary>
    /// Bancho Login response, used to tell the client whether it authenticated successfully or not
    /// Error User Ids:
    /// -1: Authentication Failed
    /// -2: Version Mismatch
    /// </summary>
    public class BanchoLoginResponse : Serializable {
        [BanchoSerialize] public int UserId;

        public BanchoLoginResponse(int userId) => this.UserId = userId;

        public Packet ToPacket() => new(RequestType.BanchoLoginReply, this);
        public static implicit operator Packet(BanchoLoginResponse response) => response.ToPacket();
    }
}
