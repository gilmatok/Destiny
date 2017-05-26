namespace Destiny
{
    public static class Constants
    {
        public const short Version = 83;
        public const string Patch = "1";
        public const byte Locale = 8;

        public static readonly byte[] RIV = new byte[] { 0x52, 0x61, 0x6A, 0x61 };
        public static readonly byte[] SIV = new byte[] { 0x6E, 0x52, 0x30, 0x58 };

        public static readonly string[] WorldNames = new string[]
        {
            "Scania", "Bera", "Broa",
            "Windia", "Khaini", "Bellocan",
            "Mardia", "Kradia", "Yellonde",
            "Demethos", "Galicia", "El Nido",
            "Zenith", "Arcania", "Chaos",
            "Nova", "Kalluna"
        };
    }

    public enum LoginResult : int
    {
        Valid,
        Banned = 3,
        IncorrectPassword,
        NotRegistered,
        SystemError,
        AlreadyLoggedIn,
        SystemError2,
        SystemError3,
        TooManyConnections,
        AgeLimit,
        NotMasterIP = 13,
        WrongGatewayInformationKorean,
        ProcessKorean,
        VerifyEmail,
        WrongGatewayInformation,
        VerifyEmail2 = 21,
        LicenceAgreement = 23,
        MapleEuropeNotice = 25,
        RequireFullVersion = 27
    }
}