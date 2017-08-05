using System;

namespace Destiny
{
    public static class Constants
    {
        public const short Version = 83;
        public const string Patch = "1";
        public const byte Locale = 8;

        public static Random Random = new Random();

        public const char CommandIndiciator = '!';

        public static readonly byte[] RIV = new byte[] { 0x52, 0x61, 0x6A, 0x61 }; // Raja
        public static readonly byte[] SIV = new byte[] { 0x6E, 0x52, 0x30, 0x58 }; // nR0X

        public static object CommandIndicator { get; internal set; }
    }

    public enum AccountLevel : byte
    {
        Normal,
        Intern,
        Gm,
        SuperGm,
        Administrator
    }

    public enum MessageType : byte
    {
        DropPickup,
        QuestRecord,
        CashItemExpire,
        IncreaseEXP,
        IncreaseFame,
        IncreaseMeso,
        IncreaseGP,
        GiveBuff,
        GeneralItemExpire,
        System,
        QuestRecordEx,
        ItemProtectExpire,
        ItemExpireReplace,
        SkillExpire
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

    public enum ServerType
    {
        Login,
        Channel,
        Shop,
        ITC
    }

    #region Character
    public enum Gender : byte
    {
        Male,
        Female,
        Both,
        Unset = 10
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
        ThunderBreaker3 = 1511,

        Legend = 2000
    }

    public enum JobType
    {
        Cygnus = 0,
        Explorer = 1,
        Aran = 2
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
    #endregion

    #region Equipment
    public enum EquipmentSlot : sbyte
    {
        Hat = -1,
        Face = -2,
        Eye = -3,
        Mantle = -4,
        Top = -5,
        Bottom = -6,
        Shoes = -7,
        Gloves = -8,
        Cape = -9,
        Shield = -10,
        Weapon = -11,
        Ring = -12,
        Necklace = -17,
        Mount = -18,
        CashHat = -101,
        CashFace = -102,
        CashEye = -103,
        CashTop = -104,
        CashOverall = -105,
        CashBottom = -106,
        CashShoes = -107,
        CashGloves = -108,
        CashCape = -109,
        CashShield = -110,
        CashWeapon = -111,
        CashRing = -112,
        CashNecklace = -117,
        CashMount = -118
    }

    public enum EquippedQueryMode
    {
        Any,
        Cash,
        Normal
    }

    public enum WeaponType
    {
        NotAWeapon,
        Bow,
        Claw,
        Dagger,
        Crossbow,
        Axe1H,
        Sword1H,
        Blunt1H,
        Axe2H,
        Sword2H,
        Blunt2H,
        PoleArm,
        Spear,
        Staff,
        Wand,
        Knuckle,
        Gun
    }
    #endregion

    #region Items
    [Flags]
    public enum ItemFlags : short
    {
        Sealed = 0x01,
        AddPreventSlipping = 0x02,
        AddPreventColdness = 0x04,
        Untradeable = 0x08,
        Scissored = 0x10
    }

    public enum ItemType : byte
    {
        Equipment = 1,
        Usable = 2,
        Setup = 3,
        Etcetera = 4,
        Cash = 5,
        Count = 6
    }
    #endregion

    #region Login
    public enum CharacterDeletionResult : byte
    {
        Valid = 0,
        InvalidPic = 20
    }

    public enum LoginResult : int
    {
        Valid = 0,
        Banned = 3,
        InvalidPassword = 4,
        InvalidUsername = 5,
        LoggedIn = 7,
        EULA = 23
    }

    public enum PinResult : byte
    {
        Valid = 0,
        Register = 1,
        Invalid = 2,
        Error = 3,
        Request = 4,
        Cancel = 5
    }
    #endregion

    #region Quests
    public enum QuestAction : byte
    {
        RestoreLostItem,
        Start,
        Complete,
        Forfeit,
        ScriptStart,
        ScriptEnd
    }

    [Flags]
    public enum QuestFlags : short
    {
        //TODO: Test this; I'm just guessing
        AutoStart = 0x01,
        SelectedMob = 0x02
    }

    public enum QuestResult : byte
    {
        AddTimeLimit = 0x06,
        RemoveTimeLimit = 0x07,
        Complete = 0x08,
        GenericError = 0x09,
        NoInventorySpace = 0x0A,
        NotEnoughMesos = 0x0B,
        ItemWornByChar = 0x0D,
        OnlyOneOfItemAllowed = 0x0E,
        Expire = 0x0F,
        ResetTimeLimit = 0x10
    }

    public enum QuestStatus : byte
    {
        NotStarted = 0,
        InProgress = 1,
        Complete = 2
    }
    #endregion

    #region NPCs
    public enum ShopAction : byte
    {
        Buy,
        Sell,
        Recharge,
        Leave
    }
    #endregion

    #region World
    public enum WorldFlag : byte
    {
        None,
        Event,
        New,
        Hot
    }

    public enum WorldNames : byte
    {
        Scania,
        Bera,
        Broa,
        Windia,
        Khaini,
        Bellocan,
        Mardia,
        Kradia,
        Yellonde,
        Demethos,
        Elnido,
        Kastia,
        Judis,
        Arkenia,
        Plana,
        Galicia,
        Kalluna,
        Stius,
        Croa,
        Zenith,
        Medere
    }

    public enum WorldStatus : short
    {
        Normal,
        HighlyPopulated,
        Full
    }

    public static class WorldNameResolver
    {
        public static byte GetID(string name)
        {
            try
            {
                return (byte)Enum.Parse(typeof(WorldNames), name.ToCamel());
            }
            catch
            {
                throw new ArgumentException("The specified World name is invalid.");
            }
        }

        public static string GetName(byte id)
        {
            try
            {
                return Enum.GetName(typeof(WorldNames), id);
            }
            catch
            {
                throw new ArgumentException("The specified World ID is invalid.");
            }
        }

        public static bool IsValid(byte id)
        {
            return Enum.IsDefined(typeof(WorldNames), id);
        }

        public static bool IsValid(string name)
        {
            try
            {
                WorldNameResolver.GetID(name);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
    #endregion
}