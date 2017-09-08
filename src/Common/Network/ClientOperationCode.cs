namespace Destiny.Network
{
    public enum ClientOperationCode : short
    {
        AccountLogin = 1,
        GuestLogin = 2,
        AccountInfo = 3,
        WorldRelist = 4,
        WorldSelect = 5,
        WorldStatus = 6,
        EULA = 7,
        AccountGender = 8,
        PinCheck = 9,
        PinUpdate = 10,
        WorldList = 11,
        LeaveCharacterSelect = 12,
        ViewAllChar = 13,
        SelectCharacterByVAC = 14,
        VACFlagSet = 15,
        //16
        //17
        //18
        CharacterSelect = 19,
        CharacterLoad = 20,
        CharacterNameCheck = 21,
        CharacterCreate = 22,
        CharacterDelete = 23,
        //24
        ClientError = 25,
        Pong = 26,
        //27
        //28
        CharacterSelectRegisterPic = 29,
        CharacterSelectRequestPic = 30,
        RegisterPicFromVAC = 31,
        RequestPicFromVAC = 32,
        //32
        //33
        //34
        ClientStart = 35,
        //36
        //37
        MapChange = 38,
        ChannelChange = 39,
        CashShopMigration = 40,
        PlayerMovement = 41,
        Sit = 42,
        UseChair = 43,
        CloseRangeAttack = 44,
        RangedAttack = 45,
        MagicAttack = 46,
        EnergyOrbAttack = 47,
        TakeDamage = 48,
        PlayerChat = 49,
        CloseChalkboard = 50,
        FaceExpression = 51,
        UseItemEffect = 52,
        UseDeathItem = 53,
        //54
        //55
        //56
        //MonsterBookCover? = 57
        NpcConverse = 58,
        //RemoteStore? = 59
        NpcResult = 60,
        NpcShop = 61,
        Storage = 62,
        HiredMerchant = 0x3F,
        //DueyAction? FredrickAction? = 0x40,
        //65
        //66
        //67
        AdminShopAction = 68,
        InventorySort = 0x45,
        InventoryGather = 0x46,
        InventoryAction = 0x47,
        UseItem = 0x48,
        CancelItemEffect = 0x49,
        UseSummonBag = 0x4B,
        UsePetFood = 0x4C,
        UseMountFood = 0x4D,
        UseScriptedItem = 0x4E,
        UseCashItem = 0x4F,

        #region QUESTIONABLE
        UseCatchItem = 0x50,
        UseSkillBook = 0x51,
        //82
        //89
        #endregion QUESTIONABLE

        UseTeleportRock = 84,
        UseReturnScroll = 85,
        UseUpgradeScroll = 86,
        DistributeAP = 87,
        AutoDistributeAP = 88,
        HealOverTime = 89,
        DistributeSP = 0x5A,
        UseSkill = 0x5B,
        CancelBuff = 0x5C,
        SkillEffect = 0x5D,
        MesoDrop = 0x5E,
        GiveFame = 0x5F,
        PlayerInformation = 97,
        SpawnPet = 0x62,
        CancelDebuff = 0x63,
        ChangeMapSpecial = 100,
        UseInnerPortal = 101,
        TrockAction = 102,
        Report = 106,

        #region QUESTIONABLE
        //103
        //104
        //106
        QuestAction = 107,
        //108
        SkillMacro = 0x6D,
        SpouseChat = 0x6E,
        UseFishingItem = 0x6F,
        MakerSkill = 0x70,
        //113
        //114
        UseRemote = 0x73,
        PartyChat = 0x74,
        //115
        //117
        //119
        //120
        //121
        //122
        #endregion QUESTIONABLE

        MultiChat = 119,
        Command = 120,
        Messenger = 122,
        PlayerInteraction = 123,
        PartyOperation = 124,
        DenyPartyRequest = 125,
        GuildOperation = 126,
        DenyGuildRequest = 127,
        AdminCommand = 128,
        AdminLog = 129,
        BuddyListModify = 130,
        NoteAction = 131,
        UseDoor = 132,
        ChangeKeymap = 135,
        FamilyPedigree = 145,
        FamilyOpen = 146,
        BbsOperation = 155,
        MovePet = 167,
        MobAutomaticProvoke = 189,
        NpcMovement = 197,

        #region QUESTIONABLE
        RingAction = 136,
        OpenFamily = 0x90,
        AddFamily = 0x91,
        AcceptFamily = 0x94,
        UseFamily = 0x95,
        AllianceOperation = 0x96,
        MtsMigration = 0x9A,
        PetTalk = 0x9B,
        UseSolomonItem = 0x9C,
        PetChat = 0xA2,
        PetCommand = 0xA3,
        PetLoot = 0xA4,
        PetAutoPot = 0xA5,
        PetExcludeItems = 0xA6,
        MoveSummon = 0xA9,
        SummonAttack = 0xAA,
        DamageSummon = 0xAB,
        Beholder = 0xAC,
        MobMovement = 0xBC,
        MobDamageModFriendly = 0xB6,
        MonsterBomb = 0xB7,
        MobDamageMob = 0xB8,
        NpcAction = 0xBB,
        DropPickup = 0xCA,
        DamageReactor = 0xC3,
        ChangedMap = 0xC4,
        MonsterCarnival = 0xD0,
        PlayerUpdate = 0xD5,
        CashShopOperation = 0xDA,
        BuyCashItem = 0xDB,
        CouponCode = 0xDC,
        LeaveField = 0xDF,
        OpenItemInterface = 0xE1,
        CloseItemInterface = 0xE2,
        UseItemInterface = 0xE3,
        MtsOperation = 0xF1,
        UseMapleLife = 0xF4,
        UseHammer = 0xF8,
        MapleTV = 0x222,
        #endregion QUESTIONABLE

        HitReactor = 205,
        TouchReactor = 206,
        //207
        PartySearchStart = 222,
        PartySearchStop = 223,
    }
}
