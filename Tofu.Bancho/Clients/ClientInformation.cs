namespace Tofu.Bancho.Clients {
    public struct ClientInformation {
        /// <summary>
        /// User Id of the CLient
        /// </summary>
        public int Id;
        /// <summary>
        /// Username of the Client
        /// </summary>
        public string Username;
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
