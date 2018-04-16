using System;

namespace Destiny.Constants
{
    public class CharacterConstants
    {
        public static class ExperienceTables
        {
            public static readonly int[] BegginerLevels = { 1, 15, 34, 57, 92, 135, 372, 560, 840, 1242, 1144 };
            public static readonly int[] FirstJobLevels = { 1573, 2144, 2800, 3640, 4700, 5893, 7360, 9144, 11120, 13477, 16268, 19320, 22880, 27008, 31477, 36600, 42444, 48720, 55813, 63800 };
            public static readonly int[] SecondJobLevels = { 86784, 98208, 110932, 124432, 139372, 155865, 173280, 192400, 213345, 235372, 259392, 285532, 312928, 342624, 374760, 408336, 445544, 483532, 524160, 567772, 598886, 631704, 666321, 702836, 741351, 781976, 824828, 870028, 917625, 967995, 1021041, 1076994, 1136013, 1198266, 1263930, 1333194, 1406252, 1483314, 1564600, 1650340 };
            public static readonly int[] ThirdJobLevels = { 1740778, 1836173, 1936794, 2042930, 2154882, 2272970, 2397528, 2528912, 2667496, 2813674, 2967863, 3130502, 3302053, 3483005, 3673873, 3875201, 4087562, 4311559, 4547832, 4797053, 5059931, 5337215, 5629694, 5938202, 6263614, 6606860, 6968915, 7350811, 7753635, 8178534, 8626718, 9099462, 9598112, 10124088, 10678888, 11264090, 11881362, 12532461, 13219239, 13943653, 14707765, 15513750, 16363902, 17260644, 18206527, 19204245, 20256637, 21366700, 22537594, 23772654 };
            public static readonly int[] FourthJobLevels = { 25075395, 26449526, 27898960, 29427822, 31040466, 32741483, 34535716, 36428273, 38424542, 40530206, 42751262, 45094030, 47565183, 50171755, 52921167, 55821246, 58880250, 62106888, 65510344, 69100311, 72887008, 76881216, 81094306, 85594273, 90225770, 95170142, 100385466, 105886589, 111689174, 117809740, 124265714, 131075474, 138258410, 145834970, 153826726, 162256430, 171148082, 180526997, 190419876, 200854885, 211861732, 223471711, 223471711, 248635353, 262260570, 276632449, 291791906, 307782102, 324648562, 342439302, 361204976, 380999008, 401877754, 423900654, 447130410, 471633156, 497478653, 524740482, 553496261, 583827855, 615821622, 649568646, 685165008, 722712050, 762316670, 804091623, 848155844, 894634784, 943660770, 995373379, 1049919840, 1107455447, 1168144006, 1232158297, 1299680571, 1370903066, 1446028554, 1525246918, 1608855764, 1697021059 };

            public static readonly int[] PetLevels = { 1, 1, 3, 6, 14, 31, 60, 108, 181, 287, 434, 632, 891, 1224, 1642, 2161, 2793, 3557, 4467, 5542, 6801, 8263, 9950, 11882, 14084, 16578, 19391, 22547, 26074, 30000, int.MaxValue };
            public static readonly int[] PetCloseness = { 0, 1, 3, 6, 14, 31, 60, 108, 181, 287, 434, 632, 891, 1224, 1642, 2161, 2793, 3557, 4467, 5542, 9000 };
            public static readonly int[] MountLevels = { 1, 24, 50, 105, 134, 196, 254, 263, 315, 367, 430, 543, 587, 679, 725, 897, 1146, 1394, 1701, 2247, 2543, 2898, 3156, 3313, 3584, 3923, 4150, 4305, 4550 };

            //TODO: these data seem slightly wrong?
            public static readonly int[] CharacterLevel = { 1, 15, 34, 57, 92, 135, 372, 560, 840, 1242, 1242, 1242, 1242, 1242, 1242, 1490, 1788, 2146, 2575, 3090, 3708, 4450, 5340, 6408, 7690, 9228, 11074, 13289, 15947, 19136, 19136, 19136, 19136, 19136, 19136, 22963, 27556, 33067, 39681, 47616, 51425, 55539, 59582, 64781, 69963, 75560, 81605, 88133, 95184, 102799, 111023, 119905, 129497, 139857, 151046, 163129, 176180, 190274, 205496, 221936, 239691, 258866, 279575, 301941, 326097, 352184, 380359, 410788, 443651, 479143, 479143, 479143, 479143, 479143, 479143, 512683, 548571, 586971, 628059, 672024, 719065, 769400, 823258, 880886, 942548, 1008526, 1079123, 1154662, 1235488, 1321972, 1414511, 1513526, 1619473, 1732836, 1854135, 1983924, 2122799, 2271395, 2430393, 2600520, 2782557, 2977336, 3185749, 3408752, 3647365, 3902680, 4175868, 4468179, 4780951, 5115618, 5473711, 5856871, 6266852, 6705531, 7176919, 7677163, 8214565, 8789584, 9404855, 10063195, 10063195, 10063195, 10063195, 10063195, 10063195, 10767619, 11521352, 12327847, 13190796, 14114152, 15102142, 16159292, 17290443, 18500774, 19795828, 21181536, 22664244, 24250741, 25948292, 27764673, 29708200, 31787774, 34012918, 36393823, 38941390, 41667310, 44584022, 47704904, 51044247, 54617344, 58440558, 62531397, 66908595, 71592197, 76603651, 81965907, 87703520, 93842766, 100411760, 107440583, 113887018, 120720239, 127963453, 135641260, 143779736, 152406520, 161550911, 171243966, 181518604, 192409720, 203954303, 216191561, 229163055, 242912838, 257487608, 272936864, 289313076, 306671861, 325072173, 344576503, 365251093, 387166159, 410396129, 435019897, 461121091, 488788356, 518115657, 549202596, 582154752, 617084037, 654109079, 693355624, 734956961, 779054379, 825797642, 875345501, 927866231, 983538205, 1042550497, 1105103527, int.MaxValue }; //NOTE: Last value is needed or else level 200 characters will throw an IndexOutOfRange exception
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

            MapleleafBrigadier = 800,
            GM = 900,
            SuperGM = 910,

            Noblesse = 1000,

            DawnWarrior1 = 1100,
            DawnWarrior2 = 1110,
            DawnWarrior3,
            DawnWarrior4,

            BlazeWizard1 = 1200,
            BlazeWizard2 = 1210,
            BlazeWizard3,
            BlazeWizard4,

            WindArcher1 = 1300,
            WindArcher2 = 1310,
            WindArcher3,
            WindArcher4,

            NightWalker1 = 1400,
            NightWalker2 = 1410,
            NightWalker3,
            NightWalker4,

            ThunderBreaker1 = 1500,
            ThunderBreaker2 = 1510,
            ThunderBreaker3,
            ThunderBreaker4,

            Aran  = 2000, // aran begginer
            Aran1 = 2100,
            Aran2 = 2110,
            Aran3,
            Aran4
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
            None = 0x0,

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

        public enum AttackElementType
        {
            Neutral,
            Fire,
            Poison,
            Ice,
            Lighting,
            Holy,
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

        public enum CharacterDisease : ulong
        {
            None = 0x0L,

            Slow = 0x1L,
            Seduce = 0x80L,
            Fishable = 0x100L,
            Confuse = 0x80000L,
            Stun = 0x2000000000000L,
            Poison = 0x4000000000000L,
            Sealed = 0x8000000000000L,
            Darkness = 0x10000000000000L,
            Weaken = 0x4000000000000000L,
            Curse = 0x8000000000000000L
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

        #region Skills and Buffs

        public static class SkillNames
        {
            public enum All : int
            {
                RegularAttack = 0
            }

            public enum Beginner : int
            {
                FollowTheLead = 8,
                BlessingOfTheFairy = 12,
                ChairMaster = 100, 
                ThreeSnails = 1000,
                Recovery = 1001,
                NimbleFeet = 1002,
                LegendarySpirit = 1003,
                MonsterRider = 1004,
                EchoOfHero = 1005,
                JumpDown = 1006, 
                Maker = 1007,
                //dojoo temporary skills
                BambooRain = 1009,
                Invincibility = 1010,
                PowerExplosion = 1011
                //TODO: other temporary skills
            }

            public enum Swordsman : int
            {
                ImprovedHPRecovery = 1000000,
                ImprovedMaxHpIncrease = 1000001,
                Endure = 1000002,
                IronBody = 1001003,
                PowerStrike = 1001004,
                SlashBlast = 1001005,
            }

            public enum Fighter : int
            {
                SwordMastery = 1100000,
                AxeMastery = 1100001,
                FinalAttackSword = 1100002,
                FinalAttackAxe = 1100003,
                SwordBooster = 1101004,
                AxeBooster = 1101005,
                Rage = 1101006,
                PowerGuard = 1101007
            }

            public enum Crusader : int
            {
                ShieldMastery = 1111001,
                ComboAttack = 1111002,
                PanicSword = 1111003,
                PanicAxe = 1111004,
                ComaSword = 1111005,
                ComaAxe = 1111006,
                ArmorCrash = 1111007,
                Shout = 1111008
            }

            public enum Hero : int
            {
                AdvancedComboAttack = 1120003,
                Achilles = 1120004,
                Guardian = 1120005,
                MapleWarrior = 1121000,
                MonsterMagnet = 1121001,
                PowerStance = 1121002,
                Rush = 1121006,
                Brandish = 1121008,
                Enrage = 1121010,
                HerosWill = 1121011,
            }

            public enum Page : int
            {
                SwordMastery = 1200000,
                BWMastery = 1200001,
                FinalAttackSword = 1200002,
                FinalAttackBW = 1200003,
                SwordBooster = 1201004,
                BWBooster = 1201005,
                Threaten = 1201006,
                PowerGuard = 1201007
            }

            public enum WhiteKnight : int
            {
                ImprovingMPRecovery = 1210000,
                ShieldMastery = 1210001,
                ChargedBlow = 1211002,
                FireChargeSword = 1211003,
                FlameChargeBW = 1211004,
                IceChargeSword = 1211005,
                BlizzardChargeBW = 1211006,
                ThunderChargeSword = 1211007,
                LightningChargeBW = 1211008,
                MagicCrash = 1211009
            }

            public enum Paladin : int
            {
                Achilles = 1220005,
                Guardian = 1220006,
                AdvancedCharge = 1220010,
                MapleWarrior = 1221000,
                MonsterMagnet = 1221001,
                PowerStance = 1221002,
                HolyChargeSword = 1221003,
                DivineChargeBW = 1221004,
                Rush = 1221007,
                Blast = 1221009,
                HeavensHammer = 1221011,
                HerosWill = 1221012
            }

            public enum Spearman : int
            {
                SpearMastery = 1300000,
                PoleArmMastery = 1300001,
                FinalAttackSpear = 1300002,
                FinalAttackPoleArm = 1300003,
                SpearBooster = 1301004,
                PoleArmBooster = 1301005,
                IronWill = 1301006,
                HyperBody = 1301007
            }

            public enum DragonKnight : int
            {
                ElementalResistance = 1310000,
                SpearCrusher = 1310001,
                PoleArmCrusher = 1310002,
                DragonFurySpear = 1311003,
                DragonFuryPoleArm = 1311004,
                Sacrifice = 1311005,
                DragonRoar = 1311006,
                PowerCrash = 1311007,
                DragonBlood = 1311008
            }

            public enum DarkKnight : int
            {
                Achilles = 1320005,
                Berserk = 1320006,
                AuraOfBeholder = 1320008,
                HexOfBeholder = 1320009,
                MapleWarrior = 1321000,
                MonsterMagnet = 1321001,
                PowerStance = 1321002,
                Rush = 1321003,
                Beholder = 1321007,
                HerosWill = 1321010
            }

            public enum Magician : int
            {
                ImprovedMPRecovery = 2000000,
                ImprovedMaxMpIncrease = 2000001,
                MagicGuard = 2001002,
                MagicArmor = 2001003,
                EnergyBolt = 2001004,
                MagicClaw = 2001005
            }

            public enum FirePoisonWizard : int
            {
                MpEater = 2100000,
                Meditation = 2101001,
                Teleportation = 2101002,
                Slow = 2101003,
                FireArrow = 2101004,
                PoisonBreath = 2101005
            }

            public enum FirePoisonMage : int
            {
                PartialResistance = 2110000,
                ElementAmplification = 2110001,
                Explosion = 2110002,
                PoisonMist = 2111003,
                Seal = 2111004,
                SpellBooster = 2111005,
                ElementComposition = 2111006
            }

            public enum FirePoisonArchMage : int
            {
                MapleWarrior = 2121000,
                BigBang = 2121001,
                ManaReflection = 2121002,
                FireDemon = 2121003,
                Infinity = 2121004,
                Elquines = 2121005,
                Paralyze = 2121006,
                MeteorShower = 2121007,
                HerosWill = 2121008
            }

            public enum IceLightningWizard : int
            {
                MpEater = 2200000,
                Meditation = 2201001,
                Teleport = 2201002,
                Slow = 2201003,
                ColdBeam = 2201004,
                ThunderBolt = 2201005
            }

            public enum IceLightningMage : int
            {
                PartialResistance = 2210000,
                ElementAmplification = 2210001,
                IceStrike = 2211002,
                ThunderSpear = 2211003,
                Seal = 2211004,
                SpellBooster = 2211005,
                ElementComposition = 2211006
            }

            public enum IceLightningArchMage : int
            {
                MapleWarrior = 2221000,
                BigBang = 2221001,
                ManaReflection = 2221002,
                IceDemon = 2221003,
                Infinity = 2221004,
                Ifrit = 2221005,
                ChainLightning = 2221006,
                Blizzard = 2221007,
                HerosWill = 2221008
            }

            public enum Cleric : int
            {
                MpEater = 2300000,
                Teleport = 2300001,
                Heal = 2301002,
                Invincible = 2301003,
                Bless = 2301004,
                HolyArrow = 2301005
            }

            public enum Priest : int
            {
                ElementalResistance = 2310000,
                Dispel = 2311001,
                MysticDoor = 2311002,
                HolySymbol = 2311003,
                ShiningRay = 2311004,
                Doom = 2311005,
                SummonDragon = 2311006
            }

            public enum Bishop : int
            {
                MapleWarrior = 2321000,
                BigBang = 2321001,
                ManaReflection = 2321002,
                Bahamut = 2321003,
                Infinity = 2321004,
                HolyShield = 2321005,
                Resurrection = 2321006,
                AngelRay = 2321007,
                Genesis = 2321008,
                HerosWill = 2321009
            }

            public enum Archer : int
            {
                TheBlessingofAmazon = 3000000,
                CriticalShot = 3000001,
                TheEyeofAmazon = 3000002,
                Focus = 3001003,
                ArrowBlow = 3001004,
                DoubleShot = 3001005
            }

            public enum Hunter : int
            {
                BowMastery = 3100000,
                FinalAttackBow = 3100001,
                BowBooster = 3101002,
                PowerKnock = 3101003,
                SoulArrowBow = 3101004,
                ArrowBombBow = 3101005
            }

            public enum Ranger : int
            {
                Thrust = 3110000,
                MortalBlow = 3110001,
                Puppet = 3111002,
                Inferno = 3111003,
                ArrowRain = 3111004,
                SilverHawk = 3111005,
                Strafe = 3111006
            }

            public enum Bowmaster : int
            {
                BowExpert = 3120005,
                MapleWarrior = 3121000,
                SharpEyes = 3121002,
                DragonsBreath = 3121003,
                Hurricane = 3121004,
                Phoenix = 3121006,
                Hamstring = 3121007,
                Concentrate = 3121008,
                HerosWill = 3121009
            }

            public enum Crossbowman : int
            {
                CrossbowMastery = 3200000,
                FinalAttackCrossbow = 3200001,
                CrossbowBooster = 3201002,
                PowerKnock = 3201002,
                SoulArrowCrossbow = 3201004,
                IronArrowCrossbow = 3201005
            }

            public enum Sniper : int
            {
                Thurst = 3210000,
                MortalBlow = 3210001,
                Puppet = 3211002,
                Blizzard = 3211003,
                ArrowEruption = 3211004,
                GoldenEagle = 3211005,
                Strafe = 3211006
            }

            public enum Marksman : int
            {
                MarksmanBoost = 3220004,
                MapleWarrior = 3221000,
                PiercingArrow = 3221001,
                SharpEyes = 3221002,
                DragonsBreath = 3221003,
                Frostprey = 3221005,
                Blind = 3221006,
                Snipe = 3221007,
                HerosWill = 3221008
            }

            public enum Rogue : int
            {
                NimbleBody = 4000000,
                KeenEyes = 4000001,
                Disorder = 4001002,
                DarkSight = 4001003,
                DoubleStab = 4001334,
                LuckySeven = 4001344
            }

            public enum Assassin : int
            {
                ClawMastery = 4100000,
                CriticalThrow = 4100001,
                Endure = 4100002,
                ClawBooster = 4101003,
                Haste = 4101004,
                Drain = 4101005
            }

            public enum Hermit : int
            {
                Alchemist = 4110000,
                MesoUp = 4111001,
                ShadowPartner = 4111002,
                ShadowWeb = 4111003,
                ShadowMeso = 4111004,
                Avenger = 4111005,
                FlashJump = 4111006
            }

            public enum NightLord : int
            {
                ShadowShifter = 4120002,
                VenomousStar = 4120005,
                MapleWarrior = 4121000,
                Taunt = 4121003,
                NinjaAmbush = 4121004,
                ShadowStars = 4121006,
                TripleThrow = 4121007,
                NinjaStorm = 4121008,
                HerosWill = 4121009
            }

            public enum Bandit : int
            {
                DaggerMastery = 4200000,
                Endure = 4201001,
                DaggerBooster = 4201002,
                Haste = 4201003,
                Steal = 4201004,
                SavageBlow = 4201005
            }

            public enum ChiefBandit : int
            {
                ShieldMastery = 4211000,
                Chakra = 4211001,
                Assaulter = 4211002,
                Pickpocket = 4211003,
                BandOfThieves = 4211004,
                MesoGuard = 4211005,
                MesoExplosion = 4211006
            }

            public enum Shadower : int
            {
                ShadowShifter = 4220002,
                VenomousStab = 4220005,
                MapleWarrior = 4221000,
                Assassinate = 4221001,
                Taunt = 4221003,
                NinjaAmbush = 4221004,
                Smokescreen = 4221006,
                BoomerangStep = 4221007,
                HerosWill = 4221008
            }

            public enum Pirate : int
            {
                BulletTime = 5000000,
                FlashFist = 5001001,
                SommersaultKick = 5001002,
                DoubleShot = 5001003,
                Dash = 5001005
            }

            public enum Brawler : int
            {
                ImproveMaxHp = 5100000,
                KnucklerMastery = 5100001,
                BackspinBlow = 5101002,
                DoubleUppercut = 5101003,
                CorkscrewBlow = 5101004,
                MPRecovery = 5101005,
                KnucklerBooster = 5101006,
                OakBarrel = 5101007
            }

            public enum Marauder : int
            {
                StunMastery = 5110000,
                EnergyCharge = 5110001,
                EnergyBlast = 5110002,
                EnergyDrain = 5111004,
                Transformation = 5111005,
                Shockwave = 5111006
            }

            public enum Buccaneer : int
            {
                MapleWarrior = 5121000,
                DragonStrike = 5121001,
                EnergyOrb = 5121002,
                SuperTransformation = 5121003,
                Demolition = 5121004,
                Snatch = 5121005,
                Barrage = 5121007,
                PiratesRage = 5121008,
                SpeedInfusion = 5121009,
                TimeLeap = 5121010
            }

            public enum Gunslinger : int
            {
                GunMastery = 5200000,
                InvisibleShot = 5201001,
                Grenade = 5201002,
                GunBooster = 5201003,
                BlankShot = 5201004,
                Wings = 5201005,
                RecoilShot = 5201006
            }

            public enum Outlaw : int
            {
                BurstFire = 5210000,
                Octopus = 5211001,
                Gaviota = 5211002,
                Flamethrower = 5211004,
                IceSplitter = 5211005,
                HomingBeacon = 5211006
            }

            public enum Corsair : int
            {
                ElementalBoost = 5220001,
                WrathoftheOctopi = 5220002,
                Bullseye = 5220011,
                MapleWarrior = 5221000,
                AerialStrike = 5221003,
                RapidFire = 5221004,
                Battleship = 5221006,
                BattleshipCannon = 5221007,
                BattleshipTorpedo = 5221008,
                Hypnotize = 5221009,
                SpeedInfusion = 5221010
            }

            public enum MapleLeafBrigadier : int
            {
                //?????? = 8001000, //lie detector
                //  ???? = 8001001  //teleport?
            }

            public enum GM : int
            {
                Haste = 9001000, //official name is Haste(Normal)
                SuperDragonRoar = 9001001,
                Teleport = 9001002,

               // Bless = 9001003,
               // Hide = 9001004,
               // Resurrection = 9001005,
               // SuperDragonRoar2 = 9001006, //??
               // Teleport2 = 9001007, //??
               // HyperBody = 9001008,
               // ADMIN_ANTIMACRO = 9001009 //??
            }

            public enum SuperGM : int
            {
                HealPlusDispel = 9101000, //official name is Heal+Dispel
                Haste = 9101001, //official name is Haste(Super)
                HolySymbol = 9101002,
                Bless = 9101003,
                Hide = 9101004,
                Resurrection = 9101005,
                SuperDragonRoar = 9101006,
                Teleport = 9101007,
                HyperBody = 9101008
            }

            public enum Noblesse : int
            {
                BlessingOfTheFairy = 10000012,
                Helper = 10000013,
                FollowtheLead = 10000018,
                ChairMastery = 10000100,
                ThreeSnails = 10001000,
                Recovery = 10001001,
                NimbleFeet = 10001002,
                LegendarySpirit = 10001003,
                MonsterRider = 10001004,
                EchoOfHero = 10001005,
                JumpDown = 10001006,
                Maker = 10001007,
                //dojoo temporary skills
                BambooThrust = 10001009,
                InvincibleBarrier = 10001010,
                MeteoShower = 10001011,
                //TODO: other temporary skills
            }

            public enum DawnWarrior : int // 1st job
            {
                MaxHPEnhancement = 11000000,
                IronBody = 11001001,
                PowerStrike = 11001002,
                SlashBlast = 11001003,
                Soul = 11001004,
            }

            public enum DawnWarrior2 : int // 2nd job
            {
                SwordMastery = 11100000,
                SwordBooster = 11101001,
                FinalAttack = 11101002,
                Rage = 11101003,
                SoulBlade = 11101004,
                SoulRush = 11101005,
            }

            public enum DawnWarrior3 : int // 3rd job
            {
                MPRecoveryRateEnhancement = 11110000,
                Advancedcombo = 11110005,
                ComboAttack = 11111001,
                Panic = 11111002,
                Coma = 11111003,
                Brandish = 11111004,
                SoulDriver = 11111006,
                SoulCharge = 11111007
            }

            public enum DawnWarrior4 : int // quest skills?
            {
            }

            public enum BlazeWizard : int
            {
                IncreasingMaxMP = 12000000,
                MagicGuard = 12001001,
                MagicArmor = 12001002,
                MagicClaw = 12001003,
                Flame = 12001004
            }

            public enum BlazeWizard2 : int
            {
                Meditation = 12101000,
                Slow = 12101001,
                FireArrow = 12101002,
                Teleport = 12101003,
                SpellBooster = 12101004,
                ElementalReset = 12101005,
                FirePillar = 12101006
            }

            public enum BlazeWizard3 : int
            {
                ElementalResistance = 12110000,
                ElementAmplification = 12110001,
                Seal = 12111002,
                MeteorShower = 12111003,
                Ifrit = 12111004,
                FlameGear = 12111005,
                FireStrike = 12111006
            }

            public enum BlazeWizard4 : int
            {
            }

            public enum WindArcher : int
            {
                CriticalShot = 13000000,
                TheEyeofAmazon = 13000001,
                Focus = 13001002,
                DoubleShot = 13001003,
                Storm = 13001004
            }

            public enum WindArcher2 : int
            {
                BowMastery = 13100000,
                Thurst = 13100004,
                BowBooster = 13101001,
                FinalAttack = 13101002,
                SoulArrow = 13101003,
                StormBreak = 13101005,
                WindWalk = 13101006
            }

            public enum WindArcher3 : int
            {
                BowExpert = 13110003,
                ArrowRain = 13111000,
                Strafe = 13111001,
                Hurricane = 13111002,
                Puppet = 13111004,
                EagleEye = 13111005,
                WindPiercing = 13111006,
                WindShot = 13111007
            }

            public enum NightWalker : int
            {
                NimbleBody = 14000000,
                KeenEyes = 14000001,
                Disorder = 14001002,
                DarkSight = 14001003,
                LuckySeven = 14001004,
                Darkness = 14001005
            }

            public enum NightWalker2 : int
            {
                ClawMastery = 14100000,
                CriticalThrow = 14100001,
                Vanish = 14100005,
                ClawBooster = 14101002,
                Haste = 14101003,
                FlashJump = 14101004,
                Vampire = 14101006
            }

            public enum NightWalker3 : int
            {
                Alchemist = 14110003,
                Venom = 14110004,
                ShadowPartner = 14111000,
                ShadowWeb = 14111001,
                Avenger = 14111002,
                Triplethrow = 14111005,
                PoisonBomb = 14111006
            }

            public enum NightWalker4 : int
            {
            }

            public enum ThunderBreaker : int
            {
                QuickMotion = 15000000,
                Straight = 15001001,
                SomersaultKick = 15001002,
                Dash = 15001003,
                LightningSprite = 15001004
            }

            public enum ThunderBreaker2 : int
            {
                ImproveMaxHP = 15100000,
                KnucklerMastery = 15100001,
                EnergyCharge = 15100004,
                KnucklerBooster = 15101002,
                CorkscrewBlow = 15101003,
                EnergyBlast = 15101005,
                LightningCharge = 15101006
            }

            public enum ThunderBreaker3 : int
            {
                CriticalPunch = 15110000,
                EnergyDrain = 15111001,
                Transformation = 15111002,
                Shockwave = 15111003,
                Barrage = 15111004,
                SpeedInfusion = 15111005,
                Spark = 15111006,
                SharkWave = 15111007,
            }

            public enum ThunderBreaker4 : int
            {
            }

            public enum Legend : int
            {
                //uninplemented 
            }

            public enum Aran : int //beginner aran
            {
                BlessingoftheFairy = 20000012,
                FollowtheLead = 20000024,
                ChairMastery = 20000100,
                ThreeSnails = 20001000,
                Recovery = 20001001,
                AgileBody = 20001002,
                LegendarySpirit = 20001003,
                MonsterRider = 20001004,
                EchoofHero = 20001005,
                JumpDown = 20001006,
                Maker = 20001007,
                //dojoo/pyramid skills
                BambooThrust = 20001009,
                InvincibleBarrier = 20001010,
                MeteoShower = 20001011,
            }

            public enum Aran1 : int //1st job
            {
                ComboAbility = 21000000,
                DoubleSwing = 21000002,
                CombatStep = 21001001,
                PolearmBooster = 21001003,
            }

            public enum Aran2 : int //2nd job
            {
                PolearmMastery = 21100000,
                TripleSwing = 21100001,
                FinalCharge = 21100002,
                ComboSmash = 21100004,
                ComboDrain = 21100005,
                BodyPressure = 21101003,
            }

            /*public enum Aran3 : int //3rd job
            {
                ComboCritical
                FullSwing
                FinalCross
                ComboFenrir
                RollingSpin
                (hidden)FullSwingDoubleSwing
                (hidden)FullSwingTripleSwing
            }

            public enum Aran4 : int //4rd job
            {
                SmartKnockback
                SnowCharge
                HighMastery
                OverSwing
                HighDefense
                FinalBlow
                ComboTempest
                ComboBarrier
                (hidden)OverSwing
                (hidden)OverSwing
                MapleWarrior
                FreezeStanding
                HerosWill =
            }*/
        }

        public enum SecondaryBuffStat : long
        {
            HomingBeacon = 0x1,
            Morph = 0x2,
            Recovery = 0x4,
            MapleWarrrior = 0x8,
            Stance = 0x10,
            SharpEyes = 0x20,
            ManaReflection = 0x40,
            ShadowClaw = 0x100,
            Infinity = 0x200,
            HolyShield = 0x400,
            Hamstring = 0x800,
            Blind = 0x1000,
            Concentrate = 0x2000,
            EchoOfHero = 0x8000,
            GhostMorph = 0x20000,
            Aura = 0x40000,
            Confuse = 0x80000,
            BerserkFury = 0x8000000,
            DivineBody = 0x10000000,
            FinalAttack = 0x80000000,
            WeaponAttack = 0x100000000L,
            WeaponDefense = 0x200000000L,
            MagicAttack = 0x400000000L,
            MagicDefense = 0x800000000L,
            Accuracy = 0x1000000000L,
            Avoid = 0x2000000000L,
            Hands = 0x4000000000L,
            Speed = 0x8000000000L,
            Jump = 0x10000000000L,
            MagicGuard = 0x20000000000L,
            DarkSight = 0x40000000000L,
            Booster = 0x80000000000L,
            PowerGuard = 0x100000000000L,
            HyperBodyHP = 0x200000000000L,
            HyperBodyMP = 0x400000000000L,
            Invincible = 0x800000000000L,
            SoulArrow = 0x1000000000000L,
            Stun = 0x2000000000000L,
            Poison = 0x4000000000000L,
            Seal = 0x8000000000000L,
            Darkness = 0x10000000000000L,
            Combo = 0x20000000000000L,
            Summon = 0x20000000000000L,
            WKCharge = 0x40000000000000L,
            DragonBlood = 0x80000000000000L,
            HolySymbol = 0x100000000000000L,
            MesoUp = 0x200000000000000L,
            ShadowPartner = 0x400000000000000L,
            PickPocket = 0x800000000000000L,
            Puppet = 0x800000000000000L,
            MesoGuard = 0x1000000000000000L,
            Weaken = 0x4000000000000000L
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
    }
}