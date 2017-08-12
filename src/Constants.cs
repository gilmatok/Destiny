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

    public static class ExperienceTables
    {
        public static readonly int[] CharacterLevel = { 1, 15, 34, 57, 92, 135, 372, 560, 840, 1242, 1242, 1242, 1242, 1242, 1242, 1490, 1788, 2146, 2575, 3090, 3708, 4450, 5340, 6408, 7690, 9228, 11074, 13289, 15947, 19136, 19136, 19136, 19136, 19136, 19136, 22963, 27556, 33067, 39681, 47616, 51425, 55539, 59582, 64781, 69963, 75560, 81605, 88133, 95184, 102799, 111023, 119905, 129497, 139857, 151046, 163129, 176180, 190274, 205496, 221936, 239691, 258866, 279575, 301941, 326097, 352184, 380359, 410788, 443651, 479143, 479143, 479143, 479143, 479143, 479143, 512683, 548571, 586971, 628059, 672024, 719065, 769400, 823258, 880886, 942548, 1008526, 1079123, 1154662, 1235488, 1321972, 1414511, 1513526, 1619473, 1732836, 1854135, 1983924, 2122799, 2271395, 2430393, 2600520, 2782557, 2977336, 3185749, 3408752, 3647365, 3902680, 4175868, 4468179, 4780951, 5115618, 5473711, 5856871, 6266852, 6705531, 7176919, 7677163, 8214565, 8789584, 9404855, 10063195, 10063195, 10063195, 10063195, 10063195, 10063195, 10767619, 11521352, 12327847, 13190796, 14114152, 15102142, 16159292, 17290443, 18500774, 19795828, 21181536, 22664244, 24250741, 25948292, 27764673, 29708200, 31787774, 34012918, 36393823, 38941390, 41667310, 44584022, 47704904, 51044247, 54617344, 58440558, 62531397, 66908595, 71592197, 76603651, 81965907, 87703520, 93842766, 100411760, 107440583, 113887018, 120720239, 127963453, 135641260, 143779736, 152406520, 161550911, 171243966, 181518604, 192409720, 203954303, 216191561, 229163055, 242912838, 257487608, 272936864, 289313076, 306671861, 325072173, 344576503, 365251093, 387166159, 410396129, 435019897, 461121091, 488788356, 518115657, 549202596, 582154752, 617084037, 654109079, 693355624, 734956961, 779054379, 825797642, 875345501, 927866231, 983538205, 1042550497, 1105103527 };
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

    public enum ForeignEffect : byte
    {
        Level = 0,
        Job = 8
    }

    public enum InteractionCode : byte
    {
        Create = 0,
        Invite = 2,
        Decline = 3,
        Visit = 4,
        Room = 5,
        Chat = 6,
        Exit = 10,
        Open = 11,
        TradeBirthday = 14,
        SetItems = 15,
        SetMeso = 16,
        Confirm = 17,
        AddItem = 22,
        Buy = 23,
        UpdateItems = 25,
        RemoveItem = 27,
        OpenStore = 30,
    }

    public enum InteractionType : byte
    {
        Omok = 1,
        Trade = 3,
        PlayerShop = 4,
        HiredMerchant = 5
    }

    public enum CommandType : byte
    {
        Find = 5,
        Whisper = 6
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

    public enum AttackType
    {
        Melee,
        Range,
        Magic,
        Summon
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

    #region Keymap
    public enum KeymapType : byte
    {
        None = 0,
        Skill = 1,
        Item = 2,
        Face = 3,
        Menu = 4,
        BasicAction = 5,
        BasicFace = 6,
        Effect = 7
    }

    public enum KeymapAction : int
    {
        EquipmentMenu = 0,
        ItemMenu = 1,
        AbilityMenu = 2,
        SkillMenu = 3,
        BuddyList = 4,
        WorldMap = 5,
        Messenger = 6,
        MiniMap = 7,
        QuestMenu = 8,
        SetKey = 9,
        AllChat = 10,
        WhisperChat = 11,
        PartyChat = 12,
        BuddyChat = 13,
        Shortcut = 14,
        QuickSlot = 15,
        ExpandChat = 16,
        GuildList = 17,
        GuildChat = 18,
        PartyList = 19,
        QuestHelper = 20,
        SpouseChat = 21,
        MonsterBook = 22,
        CashShop = 23,
        AllianceChat = 24,
        PartySearch = 25,
        FamilyList = 26,

        PickUp = 50,
        Sit = 51,
        Attack = 52,
        Jump = 53,
        NpcChat = 54,

        Cockeyed = 100,
        Happy = 101,
        Sarcastic = 102,
        Crying = 103,
        Outraged = 104,
        Shocked = 105,
        Annoyed = 106
    }

    public enum KeymapKey : int
    {
        None = 0,
        Escape = 1,
        One = 2,
        Two = 3,
        Three = 4,
        Four = 5,
        Five = 6,
        Six = 7,
        Seven = 8,
        Eight = 9,
        Nine = 10,
        Zero = 11,
        Minus = 12,
        Equals = 13,
        Backspace = 14,
        Tab = 15,
        Q = 16,
        W = 17,
        E = 18,
        R = 19,
        T = 20,
        Y = 21,
        U = 22,
        I = 23,
        O = 24,
        P = 25,
        BracketLeft = 26,
        BracketRight = 27,
        Enter = 28,
        LeftCtrl = 29,
        A = 30,
        S = 31,
        D = 32,
        F = 33,
        G = 34,
        H = 35,
        J = 36,
        K = 37,
        L = 38,
        Semicolon = 39,
        Quote = 40,
        Backtick = 41,
        LeftShift = 42,
        Backslash = 43,
        Z = 44,
        X = 45,
        C = 46,
        V = 47,
        B = 48,
        N = 49,
        M = 50,
        Comma = 51,
        Dot = 52,
        Slash = 53,
        RightShift = 54, // NOTE: Maps to LeftShift automatically
        Multiply = 55,
        LeftAlt = 56,
        Space = 57,
        CapsLock = 58,
        F1 = 59,
        F2 = 60,
        F3 = 61,
        F4 = 62,
        F5 = 63,
        F6 = 64,
        F7 = 65,
        F8 = 66,
        F9 = 67,
        F10 = 68,
        NumLock = 69,
        ScrollLock = 70,
        Numpad7 = 71,
        Numpad8 = 72,
        Numpad9 = 73,
        Subtract = 74,
        Numpad4 = 75,
        Numpad5 = 76,
        Numpad6 = 77,
        Add = 78,
        Numpad1 = 79,
        Numpad2 = 80,
        Numpad3 = 81,
        Numpad0 = 82,
        NumpadDecimal = 83,
        F11 = 87,
        F12 = 88,
        F13 = 100,
        F14 = 101,
        F15 = 102,
        JapaneseKana = 112,
        JapaneseConvert = 121,
        JapaneseNoConvert = 122,
        JapaneseYen = 125,
        NumpadEquals = 141,
        JapaneseCircumflex = 144,
        NecpcAt = 145,
        NecpcColon = 146,
        NecpcUnderline = 147,
        JapaneseKanji = 148,
        NecpcStop = 149,
        JapanAX = 150,
        J3100Unlabeled = 151,
        NumpadEnter = 156,
        RightCtrl = 157,
        Divide = 181,
        Sysrq = 183,
        RightAlt = 184,
        Pause = 197,
        Home = 199,
        ArrowUp = 200,
        PageUp = 201,
        ArrowLeft = 203,
        ArrowRight = 205,
        End = 207,
        ArrowDown = 208,
        PageDown = 209,
        Insert = 210,
        DeleteKey = 211,
        LeftWindows = 219,
        RightWindows = 220,
        Menu = 221,
        Power = 222,
        Sleep = 223
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

    public enum VACResult : byte
    {
        CharInfo = 0,
        SendCount = 1,
        AlreadyLoggedIn = 2,
        UnknownError = 3,
        NoCharacters = 4
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

    public enum StorageAction : byte
    {
        Withdraw = 4,
        Deposit,
        Unknown,
        ModifyMeso,
        Leave
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

    #region Reactors
    public enum ReactorEventType
    {
        PlainAdvanceState,
        HitFromLeft,
        HitFromRight,
        HitBySkill,
        NoClue, //NOTE: Applies to activate_by_touch reactors
        NoClue2, //NOTE: Applies to activate_by_touch reactors
        HitByItem,
        Timeout = 101
    }

    [Flags]
    public enum ReactorFlags : byte
    {
        //TODO: Test this; I'm just guessing
        FacesLeft = 0x01,
        ActivateByTouch = 0x02,
        RemoveInFieldSet = 0x04
    }
    #endregion

    #region Map
    public enum MovementType : byte
    {
        Normal = 0,
        Jump = 1,
        JumpKnockback = 2,
        Immediate = 3,
        Teleport = 4,
        Normal2 = 5,
        FlashJump = 6,
        Assaulter = 7,
        Assassinate = 8,
        Rush = 9,
        Falling = 10,
        Chair = 11,
        ExcessiveKnockback = 12,
        RecoilShot = 13,
        Unknown = 14,
        JumpDown = 15,
        Wings = 16,
        WingsFalling = 17,
        Unknown2 = 18,
        Unknown3 = 19,
        Aran = 20,
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