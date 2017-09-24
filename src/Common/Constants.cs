using System;

namespace Destiny
{
    public static class ExperienceTables
    {
        public static readonly int[] CharacterLevel = { 1, 15, 34, 57, 92, 135, 372, 560, 840, 1242, 1242, 1242, 1242, 1242, 1242, 1490, 1788, 2146, 2575, 3090, 3708, 4450, 5340, 6408, 7690, 9228, 11074, 13289, 15947, 19136, 19136, 19136, 19136, 19136, 19136, 22963, 27556, 33067, 39681, 47616, 51425, 55539, 59582, 64781, 69963, 75560, 81605, 88133, 95184, 102799, 111023, 119905, 129497, 139857, 151046, 163129, 176180, 190274, 205496, 221936, 239691, 258866, 279575, 301941, 326097, 352184, 380359, 410788, 443651, 479143, 479143, 479143, 479143, 479143, 479143, 512683, 548571, 586971, 628059, 672024, 719065, 769400, 823258, 880886, 942548, 1008526, 1079123, 1154662, 1235488, 1321972, 1414511, 1513526, 1619473, 1732836, 1854135, 1983924, 2122799, 2271395, 2430393, 2600520, 2782557, 2977336, 3185749, 3408752, 3647365, 3902680, 4175868, 4468179, 4780951, 5115618, 5473711, 5856871, 6266852, 6705531, 7176919, 7677163, 8214565, 8789584, 9404855, 10063195, 10063195, 10063195, 10063195, 10063195, 10063195, 10767619, 11521352, 12327847, 13190796, 14114152, 15102142, 16159292, 17290443, 18500774, 19795828, 21181536, 22664244, 24250741, 25948292, 27764673, 29708200, 31787774, 34012918, 36393823, 38941390, 41667310, 44584022, 47704904, 51044247, 54617344, 58440558, 62531397, 66908595, 71592197, 76603651, 81965907, 87703520, 93842766, 100411760, 107440583, 113887018, 120720239, 127963453, 135641260, 143779736, 152406520, 161550911, 171243966, 181518604, 192409720, 203954303, 216191561, 229163055, 242912838, 257487608, 272936864, 289313076, 306671861, 325072173, 344576503, 365251093, 387166159, 410396129, 435019897, 461121091, 488788356, 518115657, 549202596, 582154752, 617084037, 654109079, 693355624, 734956961, 779054379, 825797642, 875345501, 927866231, 983538205, 1042550497, 1105103527, int.MaxValue }; //NOTE: Last value is needed or else level 200 characters will throw an IndexOutOfRange exception
    }

    public static class ServerRegistrationResponseResolver
    {
        public static string Explain(ServerRegsitrationResponse Packet)
        {
            switch (Packet)
            {
                case ServerRegsitrationResponse.InvalidType:
                    return "Unknown server type.";

                case ServerRegsitrationResponse.InvalidCode:
                    return "The provided security code is not corresponding.";

                case ServerRegsitrationResponse.Full:
                    return "Cannot register as all the spots are occupied.";

                default:
                    return null;
            }
        }
    }

    public enum ServerRegsitrationResponse : byte
    {
        Valid,
        InvalidType,
        InvalidCode,
        Full
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

    public enum UserEffect : byte
    {
        LevelUp = 0,
        SkillUse = 1,
        SkillAffected = 2,
        Quest = 3,
        Pet = 4,
        SkillSpecial = 5,
        ProtectOnDieItemUse = 6,
        PlayPortalSE = 7,
        JobChanged = 8,
        QuestComplete = 9,
        IncDecHPEffect = 10,
        BuffItemEffect = 11,
        SquibEffect = 12,
        MonsterBookCardGet = 13,
        LotteryUse = 14,
        ItemLevelUp = 15,
        ItemMaker = 16,
        ExpItemConsumed = 17,
        ReservedEffect = 18,
        Buff = 19,
        ConsumeEffect = 20,
        UpgradeTombItemUse = 21,
        BattlefieldItemUse = 22,
        AvatarOriented = 23,
        IncubatorUse = 24,
        PlaySoundWithMuteBGM = 25,
        SoulStoneUse = 26,
        IncDecHPEffect_EX = 27,
        DeliveryQuestItemUse = 28, // NOTE: Post big bang update.
        RepeatEffectRemove = 29, // NOTE: Post big bang update.
        EvolRing = 30 // NOTE: Post big bang update.
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

    public enum CommandType : byte
    {
        Find = 5,
        Whisper = 6
    }

    public enum AdminCommandType : byte
    {
        CreateItem = 0,
        DestroyFirstITem = 1,
        GiveExperience = 2,
        Ban = 3,
        Block = 4,
        VarSetGet = 9,
        Hide = 16,
        ShowMessageMap = 17,
        Send = 18,
        Summon = 23,
        Snow = 28,
        Warn = 29,
        Log = 30,
        SetObjectState = 34
    }

    public enum CharacterDisease : long
    {
        None,
        Slow = 0x1,
        Seduce = 0x80,
        Fishable = 0x100,
        Confuse = 0x80000,
        Stun = 0x2000000000000,
        Poison = 0x4000000000000,
        Sealed = 0x8000000000000,
        Darkness = 0x10000000000000,
        Weaken = 0x4000000000000000
    }

    public enum ReportType : byte
    {
        IllegalProgramUsage = 0,
        ConversationClaim = 1
    }

    public enum ReportResult : byte
    {
        Success,
        UnableToLocate,
        Max10TimesADay,
        YouAreReportedByUser,
        UnknownError
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

    public enum InventoryOperationType : byte
    {
        AddItem,
        ModifyQuantity,
        ModifySlot,
        RemoveItem
    }

    public enum MemoAction : byte
    {
        Send = 0,
        Delete = 1
    }

    public enum MemoResult : byte
    {
        Send = 3,
        Sent = 4,
        Error = 5
    }

    public enum MemoError : byte
    {
        ReceiverOnline,
        ReceiverInvalidName,
        ReceiverInboxFull
    }

    public enum TrockAction : byte
    {
        Remove = 0,
        Add = 1
    }

    public enum TrockType : byte
    {
        Regular = 0,
        VIP = 1
    }

    public enum TrockResult : byte
    {
        Success,
        Unknown = 2,
        Unknown2 = 3,
        CannotGo2 = 5,
        DifficultToLocate = 6,
        DifficultToLocate2 = 7,
        CannotGo = 8,
        AlreadyThere = 9,
        CannotSaveMap = 10,
        NoobsCannotLeaveMapleIsland = 11
    }
    #endregion

    #region Interaction
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
        Medal = 27,

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

    #region Mobs
    [Flags]
    public enum MobStatus : int
    {
        None,

        WeaponAttackIcon = 0x01,
        WeaponDefenceIcon = 0x02,
        MagicAttackIcon = 0x04,
        MagicDefenceIcon = 0x08,
        AccuracyIcon = 0x10,
        AvoidabilityIcon = 0x20,
        SpeedIcon = 0x40,

        Stunned = 0x80,
        Frozen = 0x100,
        Poisoned = 0x200,
        Sealed = 0x400,

        Unknown1 = 0x800,

        WeaponAttackUp = 0x1000,
        WeaponDefenseUp = 0x2000,
        MagicAttackUp = 0x4000,
        MagicDefenseUp = 0x8000,

        Doom = 0x10000,
        ShadowWeb = 0x20000,

        WeaponImmunity = 0x40000,
        MagicImmunity = 0x80000,

        Unknown2 = 0x100000,
        Unknown3 = 0x200000,
        NinjaAmbush = 0x400000,
        Unknown4 = 0x800000,
        VenomousWeapon = 0x1000000,
        Unknown5 = 0x2000000,
        Unknown6 = 0x4000000,
        Empty = 0x8000000,
        Hypnotized = 0x10000000,
        WeaponDamageReflect = 0x20000000,
        MagicDamageReflect = 0x40000000
    }

    public enum MobSkillName : int
    {
        WeaponAttackUp = 100,
        MagicAttackUp = 101,
        WeaponDefenseUp = 102,
        MagicDefenseUp = 103,

        WeaponAttackUpAreaOfEffect = 110,
        MagicAttackUpAreaOfEffect = 111,
        WeaponDefenseUpAreaOfEffect = 112,
        MagicDefenseUpAreaOfEffect = 113,
        HealAreaOfEffect = 114,
        SpeedUpAreaOfEffect = 115,

        Seal = 120,
        Darkness = 121,
        Weakness = 122,
        Stun = 123,
        Curse = 124,
        Poison = 125,
        Slow = 126,
        Dispel = 127,
        Seduce = 128,
        SendToTown = 129,
        PoisonMist = 131,
        Confuse = 132,
        Zombify = 133,

        WeaponImmunity = 140,
        MagicImmunity = 141,
        ArmorSkill = 142,

        WeaponDamageReflect = 143,
        MagicDamageReflect = 144,
        AnyDamageReflect = 145,

        WeaponAttackUpMonsterCarnival = 150,
        MagicAttackUpMonsterCarnival = 151,
        WeaponDefenseUpMonsterCarnival = 152,
        MagicDefenseUpMonsterCarnival = 153,
        AccuracyUpMonsterCarnival = 154,
        AvoidabilityUpMonsterCarnival = 155,
        SpeedUpMonsterCarnival = 156,
        SealMonsterCarnival = 157,

        Summon = 200
    }
    #endregion

    #region NPCs
    public enum NpcMessageType : byte
    {
        Standard,
        YesNo,
        RequestText,
        RequestNumber,
        Choice,
        RequestStyle = 7,
        AcceptDecline = 12
    }

    public enum ShopAction : byte
    {
        Buy,
        Sell,
        Recharge,
        Leave
    }

    public enum AdminShopAction : byte
    {
        Buy = 1,
        Exit = 2,
        Register = 3
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

    #region Server
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
        SkillExpire,
        TutorialMessage = 23
    }

    public enum ServerType
    {
        None,
        Login,
        Channel,
        Shop,
        ITC
    }

    public enum ScriptType
    {
        Npc,
        Portal
    }

    public enum NoticeType : byte
    {
        Notice,
        Popup,
        Megaphone,
        SuperMegaphone,
        Ticker,
        Pink,
        Blue,
        ItemMegaphone = 8
    }
    #endregion

    #region Skills and Buffs
    public static class SkillNames
    {
        public enum All : int
        {
            RegularAttack = 0
        }

        public enum Beginner : int
        {
            BlessingOfTheFairy = 12,
            EchoOfHero = 1005,
            FollowTheLead = 8,
            MonsterRider = 1004,
            NimbleFeet = 1002,
            Recovery = 1001
        }

        public enum Swordsman : int
        {
            ImprovedMaxHpIncrease = 1000001,
            IronBody = 1001003
        }

        public enum Fighter : int
        {
            AxeBooster = 1101005,
            AxeMastery = 1100001,
            PowerGuard = 1101007,
            Rage = 1101006,
            SwordBooster = 1101004,
            SwordMastery = 1100000
        }

        public enum Crusader : int
        {
            ArmorCrash = 1111007,
            AxeComa = 1111006,
            AxePanic = 1111004,
            ComboAttack = 1111002,
            Shout = 1111008,
            SwordComa = 1111005,
            SwordPanic = 1111003
        }

        public enum Hero : int
        {
            Achilles = 1120004,
            AdvancedComboAttack = 1120003,
            Enrage = 1121010,
            Guardian = 1120005,
            HerosWill = 1121011,
            MapleWarrior = 1121000,
            MonsterMagnet = 1121001,
            PowerStance = 1121002
        }

        public enum Page : int
        {
            BwBooster = 1201005,
            BwMastery = 1200001,
            PowerGuard = 1201007,
            SwordBooster = 1201004,
            SwordMastery = 1200000,
            Threaten = 1201006
        }

        public enum WhiteKnight : int
        {
            BwFireCharge = 1211004,
            BwIceCharge = 1211006,
            BwLitCharge = 1211008,
            ChargeBlow = 1211002,
            MagicCrash = 1211009,
            SwordFireCharge = 1211003,
            SwordIceCharge = 1211005,
            SwordLitCharge = 1211007
        }

        public enum Paladin : int
        {
            Achilles = 1220005,
            AdvancedCharge = 1220010,
            BwHolyCharge = 1221004,
            Guardian = 1220006,
            HeavensHammer = 1221011,
            HerosWill = 1221012,
            MapleWarrior = 1221000,
            MonsterMagnet = 1221001,
            PowerStance = 1221002,
            SwordHolyCharge = 1221003
        }

        public enum Spearman : int
        {
            HyperBody = 1301007,
            IronWill = 1301006,
            PolearmBooster = 1301005,
            PolearmMastery = 1300001,
            SpearBooster = 1301004,
            SpearMastery = 1300000
        }

        public enum DragonKnight : int
        {
            DragonBlood = 1311008,
            DragonRoar = 1311006,
            ElementalResistance = 1310000,
            PowerCrash = 1311007,
            Sacrifice = 1311005
        }

        public enum DarkKnight : int
        {
            Achilles = 1320005,
            AuraOfBeholder = 1320008,
            Beholder = 1321007,
            Berserk = 1320006,
            HerosWill = 1321010,
            HexOfBeholder = 1320009,
            MapleWarrior = 1321000,
            MonsterMagnet = 1321001,
            PowerStance = 1321002
        }

        public enum Magician : int
        {
            ImprovedMaxMpIncrease = 2000001,
            MagicArmor = 2001003,
            MagicGuard = 2001002
        }

        public enum FirePoisonWizard : int
        {
            Meditation = 2101001,
            MpEater = 2100000,
            PoisonBreath = 2101005,
            Slow = 2101003
        }

        public enum FirePoisonMage : int
        {
            ElementAmplification = 2110001,
            ElementComposition = 2111006,
            PartialResistance = 2110000,
            PoisonMist = 2111003,
            Seal = 2111004,
            SpellBooster = 2111005
        }

        public enum FirePoisonArchMage : int
        {
            BigBang = 2121001,
            Elquines = 2121005,
            FireDemon = 2121003,
            HerosWill = 2121008,
            Infinity = 2121004,
            ManaReflection = 2121002,
            MapleWarrior = 2121000,
            Paralyze = 2121006
        }

        public enum IceLightningWizard : int
        {
            ColdBeam = 2201004,
            Meditation = 2201001,
            MpEater = 2200000,
            Slow = 2201003
        }

        public enum IceLightningMage : int
        {
            ElementAmplification = 2210001,
            ElementComposition = 2211006,
            IceStrike = 2211002,
            PartialResistance = 2210000,
            Seal = 2211004,
            SpellBooster = 2211005
        }

        public enum IceLightningArchMage : int
        {
            BigBang = 2221001,
            Blizzard = 2221007,
            HerosWill = 2221008,
            IceDemon = 2221003,
            Ifrit = 2221005,
            Infinity = 2221004,
            ManaReflection = 2221002,
            MapleWarrior = 2221000
        }

        public enum Cleric : int
        {
            Bless = 2301004,
            Heal = 2301002,
            Invincible = 2301003,
            MpEater = 2300000
        }

        public enum Priest : int
        {
            Dispel = 2311001,
            Doom = 2311005,
            ElementalResistance = 2310000,
            HolySymbol = 2311003,
            MysticDoor = 2311002,
            SummonDragon = 2311006
        }

        public enum Bishop : int
        {
            Bahamut = 2321003,
            BigBang = 2321001,
            HerosWill = 2321009,
            HolyShield = 2321005,
            Infinity = 2321004,
            ManaReflection = 2321002,
            MapleWarrior = 2321000,
            Resurrection = 2321006
        }

        public enum Archer : int
        {
            CriticalShot = 3000001,
            Focus = 3001003
        }

        public enum Hunter : int
        {
            ArrowBomb = 3101005,
            BowBooster = 3101002,
            BowMastery = 3100000,
            SoulArrow = 3101004
        }

        public enum Ranger : int
        {
            MortalBlow = 3110001,
            Puppet = 3111002,
            SilverHawk = 3111005
        }

        public enum Bowmaster : int
        {
            Concentrate = 3121008,
            Hamstring = 3121007,
            HerosWill = 3121009,
            Hurricane = 3121004,
            MapleWarrior = 3121000,
            Phoenix = 3121006,
            SharpEyes = 3121002
        }

        public enum Crossbowman : int
        {
            CrossbowBooster = 3201002,
            CrossbowMastery = 3200000,
            SoulArrow = 3201004
        }

        public enum Sniper : int
        {
            Blizzard = 3211003,
            GoldenEagle = 3211005,
            MortalBlow = 3210001,
            Puppet = 3211002
        }

        public enum Marksman : int
        {
            Blind = 3221006,
            Frostprey = 3221005,
            HerosWill = 3221008,
            MapleWarrior = 3221000,
            PiercingArrow = 3221001,
            SharpEyes = 3221002,
            Snipe = 3221007
        }

        public enum Rogue : int
        {
            DarkSight = 4001003,
            Disorder = 4001002,
            DoubleStab = 4001334,
            LuckySeven = 4001344
        }

        public enum Assassin : int
        {

            ClawBooster = 4101003,
            ClawMastery = 4100000,
            CriticalThrow = 4100001,
            Drain = 4101005,
            Haste = 4101004
        }

        public enum Hermit : int
        {
            Alchemist = 4110000,
            Avenger = 4111005,
            MesoUp = 4111001,
            ShadowMeso = 4111004,
            ShadowPartner = 4111002,
            ShadowWeb = 4111003
        }

        public enum NightLord : int
        {
            HerosWill = 4121009,
            MapleWarrior = 4121000,
            NinjaAmbush = 4121004,
            NinjaStorm = 4121008,
            ShadowShifter = 4120002,
            ShadowStars = 4121006,
            Taunt = 4121003,
            TripleThrow = 4121007,
            VenomousStar = 4120005
        }

        public enum Bandit : int
        {
            DaggerBooster = 4201002,
            DaggerMastery = 4200000,
            Haste = 4201003,
            SavageBlow = 4201005,
            Steal = 4201004
        }

        public enum ChiefBandit : int
        {
            Assaulter = 4211002,
            BandOfThieves = 4211004,
            Chakra = 4211001,
            MesoExplosion = 4211006,
            MesoGuard = 4211005,
            Pickpocket = 4211003
        }

        public enum Shadower : int
        {
            Assassinate = 4221001,
            BoomerangStep = 4221007,
            HerosWill = 4221008,
            MapleWarrior = 4221000,
            NinjaAmbush = 4221004,
            ShadowShifter = 4220002,
            Smokescreen = 4221006,
            Taunt = 4221003,
            VenomousStab = 4220005
        }

        public enum Pirate : int
        {
            Dash = 5001005
        }

        public enum Brawler : int
        {
            BackspinBlow = 5101002,
            CorkscrewBlow = 5101004,
            DoubleUppercut = 5101003,
            ImproveMaxHp = 5100000,
            KnucklerBooster = 5101006,
            KnucklerMastery = 5100001,
            MpRecovery = 5101005,
            OakBarrel = 5101007
        }

        public enum Marauder : int
        {
            EnergyCharge = 5110001,
            EnergyDrain = 5111004,
            StunMastery = 5110000,
            Transformation = 5111005
        }

        public enum Buccaneer : int
        {
            Demolition = 5121004,
            MapleWarrior = 5121000,
            PiratesRage = 5121008,
            Snatch = 5121005,
            SpeedInfusion = 5121009,
            SuperTransformation = 5121003,
            TimeLeap = 5121010
        }

        public enum Gunslinger : int
        {
            BlankShot = 5201004,
            Grenade = 5201002,
            GunBooster = 5201003,
            GunMastery = 5200000
        }

        public enum Outlaw : int
        {
            Flamethrower = 5211004,
            Gaviota = 5211002,
            HomingBeacon = 5211006,
            IceSplitter = 5211005,
            Octopus = 5211001
        }

        public enum Corsair : int
        {
            AerialStrike = 5221003,
            Battleship = 5221006,
            Bullseye = 5220011,
            ElementalBoost = 5220001,
            Hypnotize = 5221009,
            MapleWarrior = 5221000,
            RapidFire = 5221004,
            SpeedInfusion = 5221010,
            WrathOfTheOctopi = 5220002
        }

        public enum GM : int
        {
            Haste = 9001000,
            SuperDragonRoar = 9001001,
            Teleport = 9001007
        }

        public enum SuperGM : int
        {
            HealPlusDispel = 9101000,
            Haste = 9101001,
            HolySymbol = 9101002,
            Bless = 9101003,
            Hide = 9101004,
            Resurrection = 9101005,
            SuperDragonRoar = 9101006,
            Teleport = 9101007,
            HyperBody = 9101008,
        }

        public enum Noblesse : int
        {
            BlessingOfTheFairy = 10000012,
            EchoOfHero = 10001005,
            Maker = 10001007,
            MonsterRider = 10001004,
            NimbleFeet = 10001002,
            Recovery = 10001001
        }

        public enum DawnWarrior : int
        {
            AdvancedCombo = 11110005,
            Coma = 11111003,
            ComboAttack = 11111001,
            FinalAttack = 11101002,
            IronBody = 11001001,
            MaxHpEnhancement = 11000000,
            Panic = 11111002,
            Rage = 11101003,
            Soul = 11001004,
            SoulBlade = 11101004,
            SoulCharge = 11111007,
            SwordBooster = 11101001,
            SwordMastery = 11100000
        }

        public enum BlazeWizard : int
        {
            ElementalReset = 12101005,
            ElementAmplification = 12110001,
            FireStrike = 12111006,
            Flame = 12001004,
            FlameGear = 12111005,
            Ifrit = 12111004,
            IncreasingMaxMp = 12000000,
            MagicArmor = 12001002,
            MagicGuard = 12001001,
            Meditation = 12101000,
            Seal = 12111002,
            Slow = 12101001,
            SpellBooster = 12101004
        }

        public enum WindArcher : int
        {
            EagleEye = 13111005,
            BowBooster = 13101001,
            BowMastery = 13100000,
            CriticalShot = 13000000,
            FinalAttack = 13101002,
            Focus = 13001002,
            Hurricane = 13111002,
            Puppet = 13111004,
            SoulArrow = 13101003,
            Storm = 13001004,
            WindPiercing = 13111006,
            WindShot = 13111007,
            WindWalk = 13101006
        }

        public enum NightWalker : int
        {
            Alchemist = 14110003,
            Disorder = 14001002,
            DarkSight = 14001003,
            Darkness = 14001005,
            ClawBooster = 14101002,
            ClawMastery = 14100000,
            CriticalThrow = 14100001,
            Haste = 14101003,
            PoisonBomb = 14111006,
            ShadowPartner = 14111000,
            ShadowWeb = 14111001,
            SuddenAttack = 14100005,
            Vampire = 14101006,
            Venom = 14110004
        }

        public enum ThunderBreaker : int
        {
            CorkscrewBlow = 15101003,
            Dash = 15001003,
            EnergyCharge = 15100004,
            EnergyDrain = 15111001,
            ImproveMaxHp = 15100000,
            KnucklerBooster = 15101002,
            KnucklerMastery = 15100001,
            Lightning = 15001004,
            LightningCharge = 15101006,
            Spark = 15111006,
            SpeedInfusion = 15111005,
            Transformation = 15111002
        }
    }

    public enum SecondaryBuffStat : long
    {
        HomingBeacon = (0x1),
        Morph = (0x2),
        Recovery = (0x4),
        MapleWarrrior = (0x8),
        Stance = (0x10),
        SharpEyes = (0x20),
        ManaReflection = (0x40),
        ShadowClaw = (0x100),
        Infinity = (0x200),
        HolyShield = (0x400),
        Hamstring = (0x800),
        Blind = (0x1000),
        Concentrate = (0x2000),
        EchoOfHero = (0x8000),
        GhostMorph = (0x20000),
        Aura = (0x40000),
        Confuse = (0x80000),
        BerserkFury = (0x8000000),
        DivineBody = (0x10000000),
        FinalAttack = (0x80000000),
        WeaponAttack = (0x100000000L),
        WeaponDefense = (0x200000000L),
        MagicAttack = (0x400000000L),
        MagicDefense = (0x800000000L),
        Accuracy = (0x1000000000L),
        Avoid = (0x2000000000L),
        Hands = (0x4000000000L),
        Speed = (0x8000000000L),
        Jump = (0x10000000000L),
        MagicGuard = (0x20000000000L),
        DarkSight = (0x40000000000L),
        Booster = (0x80000000000L),
        PowerGuard = (0x100000000000L),
        HyperBodyHP = (0x200000000000L),
        HyperBodyMP = (0x400000000000L),
        Invincible = (0x800000000000L),
        SoulArrow = (0x1000000000000L),
        Stun = (0x2000000000000L),
        Poison = (0x4000000000000L),
        Seal = (0x8000000000000L),
        Darkness = (0x10000000000000L),
        Combo = (0x20000000000000L),
        Summon = (0x20000000000000L),
        WKCharge = (0x40000000000000L),
        DragonBlood = (0x80000000000000L),
        HolySymbol = (0x100000000000000L),
        MesoUp = (0x200000000000000L),
        ShadowPartner = (0x400000000000000L),
        PickPocket = (0x800000000000000L),
        Puppet = (0x800000000000000L),
        MesoGuard = (0x1000000000000000L),
        Weaken = (0x4000000000000000L),
    }

    public enum PrimaryBuffStat : long
    {
        EnergyCharged = 0,
        DashSpeed = 1,
        DashJump = 2,
        RideVehicle = 3,
        PartyBooster = 4,
        GuidedBullet = 5,
        Undead = 6
    }
    #endregion

    #region Social
    public enum MessengerAction : byte
    {
        Open = 0,
        Join = 1,
        Leave = 2,
        Invite = 3,
        Note = 4,
        Decline = 5,
        Chat = 6
    }

    public enum MessengerResult : byte
    {
        Open = 0,
        Join = 1,
        Leave = 2,
        Invite = 3,
        Note = 4,
        Decline = 5,
        Chat = 6
    }

    public enum MultiChatType : byte
    {
        Buddy = 0,
        Party = 1,
        Guild = 2,
        Alliance = 3
    }

    public enum PartyAction : byte
    {
        Create = 1,
        Leave = 2,
        Join = 3,
        Invite = 4,
        Expel = 5,
        ChangeLeader = 6
    }

    public enum PartyResult : byte
    {
        Invite = 4,
        Update = 7,
        Create = 8,
        RemoveOrLeave = 12,
        Join = 15,
        ChangeLeader = 26
    }

    public enum GuildAction : byte
    {
        Update = 0,
        Create = 2,
        Invite = 5,
        Join = 6,
        Leave = 7,
        Expel = 8,
        ModifyTitles = 13,
        ModifyRank = 14,
        ModifyEmblem = 15,
        ModifyNotice = 16
    }

    public enum GuildResult : byte
    {
        Create = 1,
        Invite = 5,
        ChangeEmblem = 17,
        Info = 26,
        AddMember = 39,
        InviteeNotInChannel = 40,
        InviteeAlreadyInGuild = 42,
        LeaveMember = 44,
        MemberExpel = 47,
        Disband = 50,
        MemberOnline = 61,
        UpdateRanks = 62,
        ChangeRank = 64,
        ShowEmblem = 66,
        UpdateNotice = 68
    }

    public enum BbsAction : byte
    {
        AddOrEdit,
        Delete,
        List,
        View,
        Reply,
        DeleteReply
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

    public enum MapTransferResult : byte
    {
        NoReason = 0,
        PortalClosed = 1,
        CannotGo = 2,
        ForceOfGround = 3,
        CannotTeleport = 4,
        ForceOfGround2 = 5,
        OnlyByParty = 6,
        CashShopNotAvailable = 7
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