namespace Tofu.Bancho.PacketObjects.Enums {
    public enum LoginResult {
        AuthFailed      = -1,
        VersionMismatch = -2,
        Banned          = -3,
        Unauthorized    = -4,
        ServerFailure   = -5,
        Succeeded       = 0,
    }
}
