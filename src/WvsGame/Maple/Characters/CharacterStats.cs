using Destiny.Network.PacketFactory.MaplePacketFactory;
using System;
using System.Collections.ObjectModel;
using static Destiny.Constants.CharacterConstants;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterStats : KeyedCollection<int, CharacterStats>
    {
        public Character Parent { get; private set; }

        public CharacterStats(Character parent)
            : base()
        {
            this.Parent = parent;
        }
        public StatisticType Statistic;

        public static bool IsAtMaxHP(Character character)
        {
            short currentHealth = character.Health;
            short characterMaxHealth = character.MaxHealth;

            return currentHealth == characterMaxHealth;
        }

        public static bool IsAtMaxMP(Character character)
        {
            short currentMana = character.Mana;
            short characterMaxMana = character.MaxMana;

            return currentMana == characterMaxMana;
        }

        // TODO: abstractify to cover anything that has HP/MP, step further any kind of power && catching/handling of errors...
        public static void AddHP(Character character, int quantity)
        {
            if (character == null) return;
            if (character.MaxHealth == short.MaxValue) return;

            character.MaxHealth += (short) quantity;
            Update(character, StatisticType.MaxHealth);
        }

        public static void AddMP(Character character, int quantity)
        {
            if (character == null) return;
            if (character.MaxMana == short.MaxValue) return;

            character.MaxMana += (short) quantity;
            Update(character, StatisticType.MaxMana);
        }

        public static void FillToFull(Character character, StatisticType typeToFill)
        {
            if (character == null) return;
            if (IsAtMaxHP(character) || IsAtMaxMP(character)) return;

            switch (typeToFill)
            {
                case StatisticType.Mana:
                    character.Health = character.MaxHealth;
                    Update(character, StatisticType.Health);
                    break;

                case StatisticType.Health:
                    character.Mana = character.MaxMana;
                    Update(character, StatisticType.Mana);
                    break;
            }
        }

        // TODO: finish this, sub-switch depending on skill and another on skill level.
        public static void AdjustHPOnLevelUp(Character character)
        {
            if (character == null) return;

            Job charJob = character.Job;
            Random r = new Random();

            switch (charJob)
            {
                case Job.Beginner:
                    short HPBonusBeginner = Convert.ToInt16(r.Next(10, 16));
                    AddHP(character, HPBonusBeginner);
                    break;

                case Job.Aran:
                    short HPBonusAran = Convert.ToInt16(r.Next(10, 16));
                    AddHP(character, HPBonusAran);
                    break;

                case Job.Noblesse:
                    short HPBonusNoblesse = Convert.ToInt16(r.Next(10, 16));
                    AddHP(character, HPBonusNoblesse);
                    break;

                case Job.Warrior:
                    short HPBonusWarrior = Convert.ToInt16(r.Next(24, 28));
                    AddHP(character, HPBonusWarrior);
                    break;

                case Job.DawnWarrior1:
                    short HPBonusDawnWarrior1 = Convert.ToInt16(r.Next(24, 28));
                    AddHP(character, HPBonusDawnWarrior1);
                    break;

                case Job.Aran1:
                    short HPBonusAran1 = Convert.ToInt16(r.Next(44, 48));
                    AddHP(character, HPBonusAran1);
                    break;

                case Job.Magician:
                    short HPBonusCrusader = Convert.ToInt16(r.Next(10, 14));
                    AddHP(character, HPBonusCrusader);
                    break;

                case Job.BlazeWizard1:
                    short HPBonusDawnWarrior2 = Convert.ToInt16(r.Next(10, 14));
                    AddHP(character, HPBonusDawnWarrior2);
                    break;

                case Job.Archer:
                    short HPBonusBowman = Convert.ToInt16(r.Next(20, 24));
                    AddHP(character, HPBonusBowman);
                    break;

                case Job.WindArcher1:
                    short HPBonusWindArcher1 = Convert.ToInt16(r.Next(20, 24));
                    AddHP(character, HPBonusWindArcher1);
                    break;

                case Job.Thief:
                    short HPBonusThief = Convert.ToInt16(r.Next(20, 24));
                    AddHP(character, HPBonusThief);
                    break;

                case Job.NightWalker1:
                    short HPBonusNightWalker1 = Convert.ToInt16(r.Next(20, 24));
                    AddHP(character, HPBonusNightWalker1);
                    break;

                case Job.Pirate:
                    short HPBonusPirate = Convert.ToInt16(r.Next(22, 28));
                    AddHP(character, HPBonusPirate);
                    break;

                case Job.ThunderBreaker1:
                    short HPBonusThunderBreaker1 = Convert.ToInt16(r.Next(22, 28));
                    AddHP(character, HPBonusThunderBreaker1);
                    break;

                case Job.GM:
                    short HPBonusGM = 30000;
                    AddHP(character, HPBonusGM);
                    break;

                case Job.SuperGM:
                    short HPBonusSuperGM = 30000;
                    AddHP(character, HPBonusSuperGM);
                    break;

                case Job.Fighter:
                    break;
                case Job.Crusader:
                    break;
                case Job.Hero:
                    break;
                case Job.Page:
                    break;
                case Job.WhiteKnight:
                    break;
                case Job.Paladin:
                    break;
                case Job.Spearman:
                    break;
                case Job.DragonKnight:
                    break;
                case Job.DarkKnight:
                    break;
                case Job.FirePoisonWizard:
                    break;
                case Job.FirePoisonMage:
                    break;
                case Job.FirePoisonArchMage:
                    break;
                case Job.IceLightningWizard:
                    break;
                case Job.IceLightningMage:
                    break;
                case Job.IceLightningArchMage:
                    break;
                case Job.Cleric:
                    break;
                case Job.Priest:
                    break;
                case Job.Bishop:
                    break;
                case Job.Hunter:
                    break;
                case Job.Ranger:
                    break;
                case Job.BowMaster:
                    break;
                case Job.CrossbowMan:
                    break;
                case Job.Sniper:
                    break;
                case Job.CrossbowMaster:
                    break;
                case Job.Assassin:
                    break;
                case Job.Hermit:
                    break;
                case Job.NightLord:
                    break;
                case Job.Bandit:
                    break;
                case Job.ChiefBandit:
                    break;
                case Job.Shadower:
                    break;
                case Job.Brawler:
                    break;
                case Job.Marauder:
                    break;
                case Job.Buccaneer:
                    break;
                case Job.Gunslinger:
                    break;
                case Job.Outlaw:
                    break;
                case Job.Corsair:
                    break;
                case Job.MapleleafBrigadier:
                    break;
                case Job.DawnWarrior2:
                    break;
                case Job.DawnWarrior3:
                    break;
                case Job.DawnWarrior4:
                    break;
                case Job.BlazeWizard2:
                    break;
                case Job.BlazeWizard3:
                    break;
                case Job.BlazeWizard4:
                    break;
                case Job.WindArcher2:
                    break;
                case Job.WindArcher3:
                    break;
                case Job.WindArcher4:
                    break;
                case Job.NightWalker2:
                    break;
                case Job.NightWalker3:
                    break;
                case Job.NightWalker4:
                    break;
                case Job.ThunderBreaker2:
                    break;
                case Job.ThunderBreaker3:
                    break;
                case Job.ThunderBreaker4:
                    break;
                case Job.Aran2:
                    break;
                case Job.Aran3:
                    break;
                case Job.Aran4:
                    break;

                default:
                    AddHP(character, 100);
                    break;
            }
        }  

        public static void AdjustMPOnLevelUp(Character character)
        {
            if (character == null) return;

            Job charJob = character.Job;
            Random r = new Random();

            switch (charJob)
            {
                case Job.Beginner:
                    short MPBonusBeginner = Convert.ToInt16(r.Next(10, 12));
                    AddMP(character, MPBonusBeginner);
                    break;

                case Job.Aran:
                    short MPBonusAran = Convert.ToInt16(r.Next(10, 12));
                    AddMP(character, MPBonusAran);
                    break;

                case Job.Noblesse:
                    short MPBonusNoblesse = Convert.ToInt16(r.Next(10, 12));
                    AddMP(character, MPBonusNoblesse);
                    break;

                case Job.Warrior:
                    short MPBonusWarrior = Convert.ToInt16(r.Next(4, 6));
                    AddMP(character, MPBonusWarrior);
                    break;

                case Job.DawnWarrior1:
                    short MPBonusDawnWarrior1 = Convert.ToInt16(r.Next(4, 6));
                    AddMP(character, MPBonusDawnWarrior1);
                    break;

                case Job.Aran1:
                    short MPBonusAran1 = Convert.ToInt16(r.Next(4, 8));
                    AddMP(character, MPBonusAran1);
                    break;

                case Job.Magician:
                    short MPBonusCrusader = Convert.ToInt16(r.Next(22, 24));
                    AddMP(character, MPBonusCrusader);
                    break;

                case Job.BlazeWizard1:
                    short MPBonusDawnWarrior2 = Convert.ToInt16(r.Next(22, 24));
                    AddMP(character, MPBonusDawnWarrior2);
                    break;

                case Job.Archer:
                    short MPBonusBowman = Convert.ToInt16(r.Next(14, 16));
                    AddMP(character, MPBonusBowman);
                    break;

                case Job.WindArcher1:
                    short MPBonusWindArcher1 = Convert.ToInt16(r.Next(14, 16));
                    AddMP(character, MPBonusWindArcher1);
                    break;

                case Job.Thief:
                    short MPBonusThief = Convert.ToInt16(r.Next(14, 16));
                    AddMP(character, MPBonusThief);
                    break;

                case Job.NightWalker1:
                    short MPBonusNightWalker1 = Convert.ToInt16(r.Next(14, 16));
                    AddMP(character, MPBonusNightWalker1);
                    break;

                case Job.Pirate:
                    short MPBonusPirate = Convert.ToInt16(r.Next(14, 16));
                    AddMP(character, MPBonusPirate);
                    break;

                case Job.ThunderBreaker1:
                    short MPBonusThunderBreaker1 = Convert.ToInt16(r.Next(14, 16));
                    AddMP(character, MPBonusThunderBreaker1);
                    break;

                case Job.GM:
                    short MPBonusGM = 30000;
                    AddMP(character, MPBonusGM);
                    break;

                case Job.SuperGM:
                    short MPBonusSuperGM = 30000;
                    AddMP(character, MPBonusSuperGM);
                    break;

                case Job.Fighter:
                    break;
                case Job.Crusader:
                    break;
                case Job.Hero:
                    break;
                case Job.Page:
                    break;
                case Job.WhiteKnight:
                    break;
                case Job.Paladin:
                    break;
                case Job.Spearman:
                    break;
                case Job.DragonKnight:
                    break;
                case Job.DarkKnight:
                    break;
                case Job.FirePoisonWizard:
                    break;
                case Job.FirePoisonMage:
                    break;
                case Job.FirePoisonArchMage:
                    break;
                case Job.IceLightningWizard:
                    break;
                case Job.IceLightningMage:
                    break;
                case Job.IceLightningArchMage:
                    break;
                case Job.Cleric:
                    break;
                case Job.Priest:
                    break;
                case Job.Bishop:
                    break;
                case Job.Hunter:
                    break;
                case Job.Ranger:
                    break;
                case Job.BowMaster:
                    break;
                case Job.CrossbowMan:
                    break;
                case Job.Sniper:
                    break;
                case Job.CrossbowMaster:
                    break;
                case Job.Assassin:
                    break;
                case Job.Hermit:
                    break;
                case Job.NightLord:
                    break;
                case Job.Bandit:
                    break;
                case Job.ChiefBandit:
                    break;
                case Job.Shadower:
                    break;
                case Job.Brawler:
                    break;
                case Job.Marauder:
                    break;
                case Job.Buccaneer:
                    break;
                case Job.Gunslinger:
                    break;
                case Job.Outlaw:
                    break;
                case Job.Corsair:
                    break;
                case Job.MapleleafBrigadier:
                    break;
                case Job.DawnWarrior2:
                    break;
                case Job.DawnWarrior3:
                    break;
                case Job.DawnWarrior4:
                    break;
                case Job.BlazeWizard2:
                    break;
                case Job.BlazeWizard3:
                    break;
                case Job.BlazeWizard4:
                    break;
                case Job.WindArcher2:
                    break;
                case Job.WindArcher3:
                    break;
                case Job.WindArcher4:
                    break;
                case Job.NightWalker2:
                    break;
                case Job.NightWalker3:
                    break;
                case Job.NightWalker4:
                    break;
                case Job.ThunderBreaker2:
                    break;
                case Job.ThunderBreaker3:
                    break;
                case Job.ThunderBreaker4:
                    break;       
                case Job.Aran2:
                    break;
                case Job.Aran3:
                    break;
                case Job.Aran4:
                    break;

                default:
                    AddMP(character, 100);
                    break;
            }
        }

        public static void GainAPOnLevelUp(Character character)
        {
            if (IsCygnus(character.Job) && character.Level <= 70)
            {
				character.AbilityPoints += 6;
            }
            else
            {
                character.AbilityPoints += 5;
            }
        }

        public static void GainSPOnLevelUp(Character character)
        {
            if (IsBeginner(character.Job))
            {
                character.SkillPoints += 1;
            }
            else
            {
                character.SkillPoints += 3;
            }
        }

        public static void LevelUP(Character character, bool PlayEffect)
        {
            // increase level & update stats
            character.Level++;
            Update(character, StatisticType.Level);
            // generate randomized HP && MP bonus
            AdjustHPOnLevelUp(character);
            AdjustMPOnLevelUp(character);
            // gain stats
            GainAPOnLevelUp(character);
            GainSPOnLevelUp(character);
            // play effect if needed
            if (PlayEffect)
				CharacterBuffs.ShowRemoteEffect(character, UserEffect.LevelUp);
        }

        public static void DistributeAP(Character character, StatisticType type, short amount = 1)
        {
            switch (type)
            {
                case StatisticType.Strength:
                    character.Strength += amount;
                    break;

                case StatisticType.Dexterity:
                    character.Dexterity += amount;
                    break;

                case StatisticType.Intelligence:
                    character.Intelligence += amount;
                    break;

                case StatisticType.Luck:
                    character.Luck += amount;
                    break;

                case StatisticType.MaxHealth:
                    // TODO: Get addition based on other factors.
                    break;

                case StatisticType.MaxMana:
                    // TODO: Get addition based on other factors.
                    break;
            }
        }
    
        public static void AddAbility(Character character, StatisticType statistic, short mod, bool isReset)
        {
            short maxStat = Int16.MaxValue; // TODO: Should this be a setting?
            bool isSubtract = mod < 0;

            lock (character)
            {
                switch (statistic)
                {
                    case StatisticType.Strength:
                        if (character.Strength >= maxStat)
                        {
                            return;
                        }

                        character.Strength += mod;
                        break;

                    case StatisticType.Dexterity:
                        if (character.Dexterity >= maxStat)
                        {
                            return;
                        }

                        character.Dexterity += mod;
                        break;

                    case StatisticType.Intelligence:
                        if (character.Intelligence >= maxStat)
                        {
                            return;
                        }

                        character.Intelligence += mod;
                        break;

                    case StatisticType.Luck:
                        if (character.Luck >= maxStat)
                        {
                            return;
                        }

                        character.Luck += mod;
                        break;

                    case StatisticType.MaxHealth:
                    case StatisticType.MaxMana:
                        {
                            // TODO: character is way too complicated for now.
                        }
                        break;
                }

                if (!isReset)
                {
                    character.AbilityPoints -= mod;
                }

                // TODO: Update bonuses.
            }
        }


        public static void Update(Character character, params StatisticType[] charStats)
        {
            character.Client.Send(MapleCharacterPackets.UpdateStatsPacket(character, charStats));  
        }

        //TODO: hp/mp modification bugs out UI bars, add multiple stats, some kind of message to sideBar/chat
        public static void giveStat(Character player, StatisticType stat, short quantity)
        {
            switch (stat)
            {
                case StatisticType.Strength:
                    int totalStrenght = player.Strength + quantity;

                    if (totalStrenght < Int16.MaxValue)
                    {
                        player.Strength += quantity;
                        Update(player, StatisticType.Strength);
                        break;
                    }

                    else
                    {
                        player.Strength = Int16.MaxValue;
                        Update(player, StatisticType.Strength);
                        break;
                    }

                case StatisticType.Dexterity:
                    int totalDexterity = player.Dexterity + quantity;

                    if (totalDexterity < Int16.MaxValue)
                    {
                        player.Dexterity += quantity;
                        Update(player, StatisticType.Dexterity);
                        break;
                    }

                    else
                    {
                        player.Dexterity = Int16.MaxValue;
                        Update(player, StatisticType.Dexterity);
                        break;
                    }

                case StatisticType.Intelligence:
                    int totalIntelligence = player.Intelligence + quantity;

                    if (totalIntelligence < Int16.MaxValue)
                    {
                        player.Intelligence += quantity;
                        Update(player, StatisticType.Intelligence);
                        break;
                    }

                    else
                    {
                        player.Intelligence = Int16.MaxValue;
                        Update(player, StatisticType.Intelligence);
                        break;
                    }

                case StatisticType.Luck:
                    int totalLuck = player.Luck + quantity;
                    

                    if (totalLuck < Int16.MaxValue)
                    {
                        player.Luck += quantity;
                        Update(player, StatisticType.Luck);
                        break;
                    }

                    else
                    {
                        player.Luck = Int16.MaxValue;
                        Update(player, StatisticType.Luck);
                        break;
                    }

                case StatisticType.Health:
                    int totalHealth = player.Health + quantity;

                    if (totalHealth < Int16.MaxValue)
                    {
                        player.Health += quantity;
                        Update(player, StatisticType.Health);
                        break;
                    }

                    else
                    {
                        player.Health = Int16.MaxValue;
                        Update(player, StatisticType.Health);
                        break;
                    }

                case StatisticType.MaxHealth:
                    int totalMaxHealth = player.MaxHealth + quantity;

                    if (totalMaxHealth < Int16.MaxValue)
                    {
                        player.MaxHealth += quantity;
                        Update(player, StatisticType.MaxHealth);
                        break;
                    }

                    else
                    {
                        player.MaxHealth = Int16.MaxValue;
                        Update(player, StatisticType.MaxHealth);
                        break;
                    }

                case StatisticType.Mana:
                    int totalMana = player.Mana + quantity;

                    if (totalMana < Int16.MaxValue)
                    {
                        player.Mana += quantity;
                        Update(player, StatisticType.Mana);
                        break;
                    }

                    else
                    {
                        player.Mana = Int16.MaxValue;
                        Update(player, StatisticType.Mana);
                        break;
                    }

                case StatisticType.MaxMana:
                    int totalMaxMana = player.MaxMana + quantity;

                    if (totalMaxMana < Int16.MaxValue)
                    {
                        player.MaxMana += quantity;
                        Update(player, StatisticType.MaxMana);
                        break;
                    }

                    else
                    {
                        player.MaxMana = Int16.MaxValue;
                        Update(player, StatisticType.MaxMana);
                        break;
                    }

                case StatisticType.AbilityPoints:
                    int totalAbilityPoints = player.AbilityPoints + quantity;

                    if (totalAbilityPoints < Int16.MaxValue)
                    {
                        player.AbilityPoints += quantity;
                        Update(player, StatisticType.AbilityPoints);
                        break;
                    }

                    else
                    {
                        player.AbilityPoints = Int16.MaxValue;
                        Update(player, StatisticType.AbilityPoints);
                        break;
                    }

                case StatisticType.SkillPoints:
                    int totalSkillPoints = player.SkillPoints + quantity;

                    if (totalSkillPoints < Int16.MaxValue)
                    {
                        player.SkillPoints += quantity;
                        Update(player, StatisticType.SkillPoints);
                        break;
                    }
                    else
                    {
                        player.SkillPoints = Int16.MaxValue;
                        Update(player, StatisticType.SkillPoints);
                        break;
                    }

                case StatisticType.Skin: break;
                case StatisticType.Face: break;
                case StatisticType.Hair: break;
                case StatisticType.Level: break;
                case StatisticType.Job: break;
                case StatisticType.Experience: break;
                case StatisticType.Fame: break;
                case StatisticType.Mesos: break;
                case StatisticType.Pet: break;
                case StatisticType.GachaponExperience: break;

                default: throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }

        protected override int GetKeyForItem(CharacterStats item)
        {
            throw new NotImplementedException();
        }
    }
}