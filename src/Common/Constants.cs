using System;

namespace Destiny
{
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
        Ring1 = -12,
        Ring2 = -13,
        // -14 ??
        Ring3 = -15,
        Ring4 = -16,
        Necklace = -17,
        Mount = -18,
        Saddle = -19,
        Medal = -49,
        Belt = -50,
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

        ShowDown = 0x800,

        WeaponAttackUp = 0x1000,
        WeaponDefenseUp = 0x2000,
        MagicAttackUp = 0x4000,
        MagicDefenseUp = 0x8000,

        Doom = 0x10000,
        ShadowWeb = 0x20000,

        WeaponImmunity = 0x40000,
        MagicImmunity = 0x80000,

        Unknown2 = 0x100000,
        HardSkin = 0x200000,
        NinjaAmbush = 0x400000,
        ElementalAttribute = 0x800000, 
        VenomousWeapon = 0x1000000,
        Blind = 0x2000000,
        SealSkill = 0x4000000,
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
        ScrollingText,
        PinkText,
        LightBlueText,
        // 7
        ItemMegaphone = 8,
        // 9
        // 10
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