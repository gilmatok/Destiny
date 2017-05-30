using System;

namespace Destiny
{
    public static class Constants
    {
        public const short Version = 83;
        public const string Patch = "1";
        public const byte Locale = 8;

        public const char CommandIndiciator = '!';

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

    public enum GmLevel : byte
    {
        Intern,
        Gm,
        SuperGm,
        Administrator
    }

    public enum Gender : byte
    {
        Male,
        Female,
        Both
    }

    public enum NoticeType : byte
    {
        Notice,
        Popup,
        Background,
        Ticker = 4,
        Pink,
        Blue
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

    [Flags]
    public enum StatisticType : int
    {
        Skin = 0x1,
        Face = 0x2,
        Hair = 0x4,
        Level = 0x10,
        Job = 0x20,
        Strength = 0x40,
        Dexterity = 0x80,
        Intelligence = 0x100,
        Luck = 0x200,
        Health = 0x400,
        MaxHealth = 0x800,
        Mana = 0x1000,
        MaxMana = 0x2000,
        AbilityPoints = 0x4000,
        SkillPoints = 0x8000,
        Experience = 0x10000,
        Fame = 0x20000,
        Mesos = 0x40000,
        Pet = 0x180000,
        GachaponExperience = 0x200000
    }

    public enum ItemType : byte
    {
        Equipment = 1,
        Usable,
        Setup,
        Etcetera,
        Cash
    }

    public enum InventoryType : byte
    {
        Equipment,
        Usable,
        Setup,
        Etcetera,
        Cash,
        Count
    }

    public enum EquipmentSlot : byte
    {
        Helm = 1,
        Face = 2,
        Eye = 3,
        Earring = 4,
        Top = 5,
        Bottom = 6,
        Shoe = 7,
        Glove = 8,
        Cape = 9,
        Shield = 10,
        Weapon = 11,
        FirstRelationshipRing = 12,
        LabelRing = 13,
        SecondRelationshipRing = 14,
        QuoteRing = 15,
        Pendant = 17,
        Mount = 18,
        Saddle = 19,

        Count = 100
    }

    public enum ItemKind : byte
    {
        ArmorHelm = 100,
        ArmorFace = 101,
        ArmorEyes = 102,
        ArmorEarring = 103,
        ArmorShirt = 104,
        ArmorOverall = 105,
        ArmorPants = 106,
        ArmorShoes = 107,
        ArmorGloves = 108,
        ArmorShield = 109,
        ArmorCape = 110,
        ArmorRing = 111,
        ArmorPendant = 112,
        Weapon1hSword = 130,
        Weapon1hAxe = 131,
        Weapon1hMace = 132,
        WeaponDagger = 133,
        WeaponWand = 137,
        WeaponStaff = 138,
        Weapon2HSword = 140,
        Weapon2HAxe = 141,
        Weapon2HMace = 142,
        WeaponSpear = 143,
        WeaponPolearm = 144,
        WeaponBow = 145,
        WeaponCrossbow = 146,
        WeaponClaw = 147,
        WeaponKnuckle = 148,
        WeaponGun = 149,
        Mount = 190,
        Arrow = 206,
        Star = 207,
        Bullet = 233,
        MonsterCard = 238
    }
}