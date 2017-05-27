namespace Destiny
{
    public static class Constants
    {
        public const short Version = 83;
        public const string Patch = "1";
        public const byte Locale = 8;

        public static readonly byte[] RIV = new byte[] { 0x52, 0x61, 0x6A, 0x61 };
        public static readonly byte[] SIV = new byte[] { 0x6E, 0x52, 0x30, 0x58 };
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

    public enum WorldFlag : byte
    {
        None,
        Event,
        New,
        Hot
    }

    public enum WorldStatus : short
    {
        Normal,
        HighlyPopulated,
        Full
    }

    public enum Gender : byte
    {
        Male,
        Female,
        Both
    }

     public enum Job : short
    {
        Beginner,

        Warrior = 100,
        Fighter = 110,
        Crusader,
        Hero,
        Page = 120,
        WhiteKnight,
        Paladin,
        Spearman = 130,
        DragonKnight,
        DarkKnight,

        Magician = 200,
        FirePoisonWizard = 210,
        FirePoisonMage,
        FirePoisonArchMage,
        IceLightningWizard = 220,
        IceLightningMage,
        IceLightningArchMage,
        Cleric = 230,
        Priest,
        Bishop,

        Bowman = 300,
        Hunter = 310,
        Ranger,
        BowMaster,
        CrossbowMan = 320,
        Sniper,
        CrossbowMaster,

        Thief = 400,
        Assassin = 410,
        Hermit,
        NightLord,
        Bandit = 420,
        ChiefBandit,
        Shadower,

        Pirate = 500,
        Brawler = 510,
        Marauder,
        Buccaneer,
        Gunslinger = 520,
        Outlaw,
        Corsair,

        GM = 900,
        SuperGM = 910,

        Noblesse = 1000,

        DawnWarrior1 = 1100,
        DawnWarrior2 = 1110,
        DawnWarrior3 = 1111,

        BlazeWizard1 = 1200,
        BlazeWizard2 = 1210,
        BlazeWizard3 = 1211,

        WindArcher1 = 1300,
        WindArcher2 = 1310,
        WindArcher3 = 1311,

        NightWalker1 = 1400,
        NightWalker2 = 1410,
        NightWalker3 = 1411,

        ThunderBreaker1 = 1500,
        ThunderBreaker2 = 1510,
        ThunderBreaker3 = 1511
    }
}