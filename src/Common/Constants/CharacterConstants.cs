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

            Legend = 2000,

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
                ChairMaster = 100, //hidden?
                ThreeSnails = 1000,
                Recovery = 1001,
                NimbleFeet = 1002,
                LegendarySpirit = 1003,
                MonsterRider = 1004,
                EchoOfHero = 1005,
                JumpDown = 1006, //hidden?
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
                ElementComposition = 2111006,
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
                TheBlessingofAmazon = 3000000,
                CriticalShot = 3000001,
                TheEyeofAmazon = 3000002,
                Focus = 3001003,
                ArrowBlow = 3001004,
                DoubleShot = 3001005
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
                NimbleBody = 4000000,
                KeenEyes = 4000001,
                Disorder = 4001002,
                DarkSight = 4001003,
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
                MpRecovery = 5101005,
                KnucklerBooster = 5101006,
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

            public enum MapleLeafBrigadier : int
            {
                //?????? = 8001000,
                //  ???? = 8001001
            }

            public enum GM : int
            {
                Haste = 9001000, //official name is Haste(Normal)
                SuperDragonRoar = 9001001,
                Teleport = 9001002, //this is different teleport from superGM
                Bless = 9001003,
                Hide = 9001004,
                Resurrection = 9001005,
                SuperDragonRoar2 = 9001006, //??
                Teleport2 = 9001007, //??
                HyperBody = 9001008,
                ADMIN_ANTIMACRO = 9001009 //??
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
                JumpDown = 10001006, //hidden?
                Maker = 10001007,
                //dojoo temporary skills
                BambooThrust= 10001009,
                InvincibleBarrier = 10001010,
                MeteoShower = 10001011,
                //TODO: other temporary skills
            }

            public enum DawnWarrior : int
            {
                MaxHPEnhancement = 11000000,
                IronBody = 11001001,
                PowerStrike = 11001002,
                SlashBlast = 11001003,
                Soul = 11001004,
                SwordMastery = 11100000,
                SwordBooster = 11101001,
                FinalAttack = 11101002,
                Rage = 11101003,
                SoulBlade = 11101004,
                SoulRush = 11101005,
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
            Weaken = 0x4000000000000000L,
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
