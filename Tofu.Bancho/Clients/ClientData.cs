namespace Tofu.Bancho.Clients {
    public enum ClientType {
        Irc,
        Build282
    }

    public class ClientData {
        public byte       Timezone;
        public ClientType ClientType;
    }
}
