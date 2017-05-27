using System;

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
        Cash
    }

    [Flags]
    public enum NpcFlags : byte
    {
        None = 0 << 0,
        MapleTV = 1 << 0,
        IsGuildRank = 1 << 1
    }

    [Flags]
    public enum ReactorFlags
    {
        None = 0 << 0,
        ActivateByTouch = 1 << 0,
        RemoveInFieldSet = 1 << 1
    }

    public enum ReactorStateType : byte
    {
        None = 0,
        PlainAdvanceState = 1,
        HitFromLeft = 2,
        HitFromRight = 3,
        HitBySkill = 4,
        NoClue = 5,
        NoClue2 = 6,
        HitByItem = 7
    }

    [Flags]
    public enum ReactorDropFlags
    {
        None = 0 << 0,
        IsMesos = 1 << 0
    }

    [Flags]
    public enum MobFlags : ushort
    {
        None = 0 << 0,
        Boss = 1 << 0,
        Undead = 1 << 1,
        Flying = 1 << 2,
        Friendly = 1 << 3,
        PublicReward = 1 << 4,
        ExplosiveReward = 1 << 5,
        Invincible = 1 << 6,
        AutoAggro = 1 << 7,
        DamagedByNormalAttacksOnly = 1 << 8,
        NoRemoveOnDeath = 1 << 9,
        CannotDamagePlayer = 1 << 10,
        PlayerCannotDamage = 1 << 11
    }

    public enum MobMagicModifier : byte
    {
        Immune = 1,
        Normal = 2,
        Strong = 3,
        Weak = 4
    }

    [Flags]
    public enum MobAttackFlags : byte
    {
        None = 0 << 0,
        Deadly = 1 << 0,
        Magic = 2 << 0,
        Knockback = 3 << 0,
        Jumpable = 4 << 0
    }

    [Flags]
    public enum MobDropFlags
    {
        None = 0 << 0,
        IsMesos = 1 << 0
    }

    [Flags]
    public enum ItemFlags : ushort
    {
        None = 0 << 0,
        TimeLimited = 1 << 0,
        NoTrade = 1 << 1,
        NoSale = 1 << 2,
        KarmaScissorable = 1 << 3,
        ExpireOnLogout = 1 << 4,
        BlockPickup = 1 << 5,
        Quest = 1 << 6,
        Cash = 1 << 7,
        PartyQuest = 1 << 8
    }

    [Flags]
    public enum EMapFlags : ushort
    {
        None = 0 << 0,
        Town = 1 << 0,
        Clock = 1 << 1,
        Swim = 1 << 2,
        Fly = 1 << 3,
        Everlast = 1 << 4,
        No_Party_Leader_Pass = 1 << 5,
        NoPartyLeaderPass = 1 << 5,
        Shop = 1 << 6,
        Scroll_Disable = 1 << 7,
        ScrollDisable = 1 << 7,
        Shuffle_Reactors = 1 << 8,
        ShuffleReactors = 1 << 8
    }

    [Flags]
    public enum EMapFieldType : ushort
    {
        None = 0 << 0,
        No_Clue1 = 1 << 0,
        No_Clue2 = 1 << 1,
        No_Clue3 = 1 << 2,
        No_Clue4 = 1 << 3,
        No_Clue5 = 1 << 4,
        No_Clue6 = 1 << 5,
        Force_Map_Equip = 1 << 6,
        ForceMapEquip = 1 << 6,
        No_Clue7 = 1 << 7,
        No_Clue8 = 1 << 8,
        No_Clue9 = 1 << 9
    }

    [Flags]
    public enum EMapFieldLimit : long
    {
        None = 0 << 0,
        Jump = 1 << 0,
        Movement_Skills = 1 << 1,
        MovementSkills = 1 << 1,
        Summoning_Bag = 1 << 2,
        SummoningBag = 1 << 2,
        Mystic_Door = 1 << 3,
        MysticDoor = 1 << 3,
        Channel_Switching = 1 << 4,
        ChannelSwitching = 1 << 4,
        Regular_EXP_Loss = 1 << 5,
        RegularEXPLoss = 1 << 5,
        VIP_Rock = 1 << 6,
        VIPRock = 1 << 6,
        MiniGames = 1 << 7,
        No_Clue1 = 1 << 8,
        Mount = 1 << 9,
        No_Clue2 = 1 << 10,
        No_Clue3 = 1 << 11,
        Potion_Use = 1 << 12,
        PotionUse = 1 << 12,
        No_Clue4 = 1 << 13,
        Unused1 = 1 << 14,
        No_Clue5 = 1 << 15,
        No_Clue6 = 1 << 16,
        Drop_Down = 1 << 17,
        DropDown = 1 << 17,
        No_Clue7 = 1 << 18,
        No_Clue8 = 1 << 19,
        Unused2 = 1 << 20,
        Unused3 = 1 << 21,
        Chalkboard = 1 << 22,
        Unused4 = 1 << 23,
        Unused5 = 1 << 24,
        Unused6 = 1 << 25,
        Unused7 = 1 << 26,
        Unused8 = 1 << 27,
        Unused9 = 1 << 28,
        Unused10 = 1 << 29,
        Unused11 = 1 << 30,
        Unused12 = 1 << 31,
    }
}