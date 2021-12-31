using Tofu.Bancho.Helpers.BanchoSerializer;
using Tofu.Bancho.PacketObjects;
using Tofu.Bancho.Packets.Build282.Enums;

namespace Tofu.Bancho.Packets.Build282 {
    public class BanchoSendIrcMessage : Serializable {
        [BanchoSerialize] public string Sender;
        [BanchoSerialize] public string Message;

        public static BanchoSendIrcMessage Create(Message message) => new BanchoSendIrcMessage {
            Sender = message.Sender, Message = $"<{message.Target}> {message.Content}"
        };

        public Packet ToPacket() => new(RequestType.BanchoSendIrcMessage, this);

        public static implicit operator Packet(BanchoSendIrcMessage response) => response.ToPacket();
    }
}
