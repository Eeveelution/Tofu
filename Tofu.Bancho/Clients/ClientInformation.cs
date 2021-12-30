using Tofu.Bancho.DatabaseObjects;

namespace Tofu.Bancho.Clients {
    public struct ClientInformation {
        /// <summary>
        /// Everything known about the User
        /// </summary>
        public User User;
        /// <summary>
        /// User Id of the CLient
        /// </summary>
        public int Id => (int) User.Id;
        /// <summary>
        /// Username of the Client
        /// </summary>
        public string Username => User.Username;
        /// <summary>
        /// Timezone of the Client
        /// </summary>
        public int Timezone;
        /// <summary>
        /// Whether the Login succeeded prior to Upgrading
        /// </summary>
        public bool LoginSuccessPending;
        /// <summary>
        /// osu! Presence Information
        /// </summary>
        public OsuPresence Presence;
    }
}
