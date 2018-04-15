using System;
using System.Collections.ObjectModel;
using Destiny.Constants;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterStats : KeyedCollection<int, CharacterStats>
    {
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
            character.Update(CharacterConstants.StatisticType.MaxHealth);
        }

        public static void AddMP(Character character, int quantity)
        {
            if (character == null) return;
            if (character.MaxMana == short.MaxValue) return;

            character.MaxMana += (short) quantity;
            character.Update(CharacterConstants.StatisticType.MaxMana);
        }

        public static void FillToFull(Character character, CharacterConstants.StatisticType typeToFill)
        {
            if (character == null) return;
            if (IsAtMaxHP(character) || IsAtMaxMP(character)) return;

            switch (typeToFill)
            {
                case CharacterConstants.StatisticType.Mana:
                    character.Health = character.MaxHealth;
                    character.Update(CharacterConstants.StatisticType.Health);
                    break;

                case CharacterConstants.StatisticType.Health:
                    character.Mana = character.MaxMana;
                    character.Update(CharacterConstants.StatisticType.Mana);
                    break;
            }
        }

        // TODO: finish this, sub-switch depending on skill and another on skill level.
        public static void AdjustHPOnLevelUP(Character character)
        {
            if (character == null) return;

            CharacterConstants.Job charJob = character.Job;
            Random r = new Random();

            switch (charJob)
            {
                case CharacterConstants.Job.Beginner:
                    short HPBonusBeginner = Convert.ToInt16(r.Next(10, 16));
                    AddHP(character, HPBonusBeginner);
                    break;

                case CharacterConstants.Job.Aran:
                    short HPBonusAran = Convert.ToInt16(r.Next(10, 16));
                    AddHP(character, HPBonusAran);
                    break;

                case CharacterConstants.Job.Noblesse:
                    short HPBonusNoblesse = Convert.ToInt16(r.Next(10, 16));
                    AddHP(character, HPBonusNoblesse);
                    break;

                case CharacterConstants.Job.Warrior:
                    short HPBonusWarrior = Convert.ToInt16(r.Next(24, 28));
                    AddHP(character, HPBonusWarrior);
                    break;

                case CharacterConstants.Job.DawnWarrior1:
                    short HPBonusDawnWarrior1 = Convert.ToInt16(r.Next(24, 28));
                    AddHP(character, HPBonusDawnWarrior1);
                    break;

                case CharacterConstants.Job.Aran1:
                    short HPBonusAran1 = Convert.ToInt16(r.Next(44, 48));
                    AddHP(character, HPBonusAran1);
                    break;

                case CharacterConstants.Job.Magician:
                    short HPBonusCrusader = Convert.ToInt16(r.Next(10, 14));
                    AddHP(character, HPBonusCrusader);
                    break;

                case CharacterConstants.Job.BlazeWizard1:
                    short HPBonusDawnWarrior2 = Convert.ToInt16(r.Next(10, 14));
                    AddHP(character, HPBonusDawnWarrior2);
                    break;

                case CharacterConstants.Job.Bowman:
                    short HPBonusBowman = Convert.ToInt16(r.Next(20, 24));
                    AddHP(character, HPBonusBowman);
                    break;

                case CharacterConstants.Job.WindArcher1:
                    short HPBonusWindArcher1 = Convert.ToInt16(r.Next(20, 24));
                    AddHP(character, HPBonusWindArcher1);
                    break;

                case CharacterConstants.Job.Thief:
                    short HPBonusThief = Convert.ToInt16(r.Next(20, 24));
                    AddHP(character, HPBonusThief);
                    break;

                case CharacterConstants.Job.NightWalker1:
                    short HPBonusNightWalker1 = Convert.ToInt16(r.Next(20, 24));
                    AddHP(character, HPBonusNightWalker1);
                    break;

                case CharacterConstants.Job.Pirate:
                    short HPBonusPirate = Convert.ToInt16(r.Next(22, 28));
                    AddHP(character, HPBonusPirate);
                    break;

                case CharacterConstants.Job.ThunderBreaker1:
                    short HPBonusThunderBreaker1 = Convert.ToInt16(r.Next(22, 28));
                    AddHP(character, HPBonusThunderBreaker1);
                    break;

                case CharacterConstants.Job.GM:
                    short HPBonusGM = 30000;
                    AddHP(character, HPBonusGM);
                    break;

                case CharacterConstants.Job.SuperGM:
                    short HPBonusSuperGM = 30000;
                    AddHP(character, HPBonusSuperGM);
                    break;

                case CharacterConstants.Job.Fighter:
                    break;
                case CharacterConstants.Job.Crusader:
                    break;
                case CharacterConstants.Job.Hero:
                    break;
                case CharacterConstants.Job.Page:
                    break;
                case CharacterConstants.Job.WhiteKnight:
                    break;
                case CharacterConstants.Job.Paladin:
                    break;
                case CharacterConstants.Job.Spearman:
                    break;
                case CharacterConstants.Job.DragonKnight:
                    break;
                case CharacterConstants.Job.DarkKnight:
                    break;
                case CharacterConstants.Job.FirePoisonWizard:
                    break;
                case CharacterConstants.Job.FirePoisonMage:
                    break;
                case CharacterConstants.Job.FirePoisonArchMage:
                    break;
                case CharacterConstants.Job.IceLightningWizard:
                    break;
                case CharacterConstants.Job.IceLightningMage:
                    break;
                case CharacterConstants.Job.IceLightningArchMage:
                    break;
                case CharacterConstants.Job.Cleric:
                    break;
                case CharacterConstants.Job.Priest:
                    break;
                case CharacterConstants.Job.Bishop:
                    break;
                case CharacterConstants.Job.Hunter:
                    break;
                case CharacterConstants.Job.Ranger:
                    break;
                case CharacterConstants.Job.BowMaster:
                    break;
                case CharacterConstants.Job.CrossbowMan:
                    break;
                case CharacterConstants.Job.Sniper:
                    break;
                case CharacterConstants.Job.CrossbowMaster:
                    break;
                case CharacterConstants.Job.Assassin:
                    break;
                case CharacterConstants.Job.Hermit:
                    break;
                case CharacterConstants.Job.NightLord:
                    break;
                case CharacterConstants.Job.Bandit:
                    break;
                case CharacterConstants.Job.ChiefBandit:
                    break;
                case CharacterConstants.Job.Shadower:
                    break;
                case CharacterConstants.Job.Brawler:
                    break;
                case CharacterConstants.Job.Marauder:
                    break;
                case CharacterConstants.Job.Buccaneer:
                    break;
                case CharacterConstants.Job.Gunslinger:
                    break;
                case CharacterConstants.Job.Outlaw:
                    break;
                case CharacterConstants.Job.Corsair:
                    break;
                case CharacterConstants.Job.MapleleafBrigadier:
                    break;
                case CharacterConstants.Job.DawnWarrior2:
                    break;
                case CharacterConstants.Job.DawnWarrior3:
                    break;
                case CharacterConstants.Job.DawnWarrior4:
                    break;
                case CharacterConstants.Job.BlazeWizard2:
                    break;
                case CharacterConstants.Job.BlazeWizard3:
                    break;
                case CharacterConstants.Job.BlazeWizard4:
                    break;
                case CharacterConstants.Job.WindArcher2:
                    break;
                case CharacterConstants.Job.WindArcher3:
                    break;
                case CharacterConstants.Job.WindArcher4:
                    break;
                case CharacterConstants.Job.NightWalker2:
                    break;
                case CharacterConstants.Job.NightWalker3:
                    break;
                case CharacterConstants.Job.NightWalker4:
                    break;
                case CharacterConstants.Job.ThunderBreaker2:
                    break;
                case CharacterConstants.Job.ThunderBreaker3:
                    break;
                case CharacterConstants.Job.ThunderBreaker4:
                    break;
                case CharacterConstants.Job.Aran2:
                    break;
                case CharacterConstants.Job.Aran3:
                    break;
                case CharacterConstants.Job.Aran4:
                    break;

                default:
                    AddHP(character, 100);
                    break;
            }
        }  

        public static void AdjustMPOnLevelUP(Character character)
        {
            if (character == null) return;

            CharacterConstants.Job charJob = character.Job;
            Random r = new Random();

            switch (charJob)
            {
                case CharacterConstants.Job.Beginner:
                    short MPBonusBeginner = Convert.ToInt16(r.Next(10, 12));
                    AddMP(character, MPBonusBeginner);
                    break;

                case CharacterConstants.Job.Aran:
                    short MPBonusAran = Convert.ToInt16(r.Next(10, 12));
                    AddMP(character, MPBonusAran);
                    break;

                case CharacterConstants.Job.Noblesse:
                    short MPBonusNoblesse = Convert.ToInt16(r.Next(10, 12));
                    AddMP(character, MPBonusNoblesse);
                    break;

                case CharacterConstants.Job.Warrior:
                    short MPBonusWarrior = Convert.ToInt16(r.Next(4, 6));
                    AddMP(character, MPBonusWarrior);
                    break;

                case CharacterConstants.Job.DawnWarrior1:
                    short MPBonusDawnWarrior1 = Convert.ToInt16(r.Next(4, 6));
                    AddMP(character, MPBonusDawnWarrior1);
                    break;

                case CharacterConstants.Job.Aran1:
                    short MPBonusAran1 = Convert.ToInt16(r.Next(4, 8));
                    AddMP(character, MPBonusAran1);
                    break;

                case CharacterConstants.Job.Magician:
                    short MPBonusCrusader = Convert.ToInt16(r.Next(22, 24));
                    AddMP(character, MPBonusCrusader);
                    break;

                case CharacterConstants.Job.BlazeWizard1:
                    short MPBonusDawnWarrior2 = Convert.ToInt16(r.Next(22, 24));
                    AddMP(character, MPBonusDawnWarrior2);
                    break;

                case CharacterConstants.Job.Bowman:
                    short MPBonusBowman = Convert.ToInt16(r.Next(14, 16));
                    AddMP(character, MPBonusBowman);
                    break;

                case CharacterConstants.Job.WindArcher1:
                    short MPBonusWindArcher1 = Convert.ToInt16(r.Next(14, 16));
                    AddMP(character, MPBonusWindArcher1);
                    break;

                case CharacterConstants.Job.Thief:
                    short MPBonusThief = Convert.ToInt16(r.Next(14, 16));
                    AddMP(character, MPBonusThief);
                    break;

                case CharacterConstants.Job.NightWalker1:
                    short MPBonusNightWalker1 = Convert.ToInt16(r.Next(14, 16));
                    AddMP(character, MPBonusNightWalker1);
                    break;

                case CharacterConstants.Job.Pirate:
                    short MPBonusPirate = Convert.ToInt16(r.Next(14, 16));
                    AddMP(character, MPBonusPirate);
                    break;

                case CharacterConstants.Job.ThunderBreaker1:
                    short MPBonusThunderBreaker1 = Convert.ToInt16(r.Next(14, 16));
                    AddMP(character, MPBonusThunderBreaker1);
                    break;

                case CharacterConstants.Job.GM:
                    short MPBonusGM = 30000;
                    AddMP(character, MPBonusGM);
                    break;

                case CharacterConstants.Job.SuperGM:
                    short MPBonusSuperGM = 30000;
                    AddMP(character, MPBonusSuperGM);
                    break;

                case CharacterConstants.Job.Fighter:
                    break;
                case CharacterConstants.Job.Crusader:
                    break;
                case CharacterConstants.Job.Hero:
                    break;
                case CharacterConstants.Job.Page:
                    break;
                case CharacterConstants.Job.WhiteKnight:
                    break;
                case CharacterConstants.Job.Paladin:
                    break;
                case CharacterConstants.Job.Spearman:
                    break;
                case CharacterConstants.Job.DragonKnight:
                    break;
                case CharacterConstants.Job.DarkKnight:
                    break;
                case CharacterConstants.Job.FirePoisonWizard:
                    break;
                case CharacterConstants.Job.FirePoisonMage:
                    break;
                case CharacterConstants.Job.FirePoisonArchMage:
                    break;
                case CharacterConstants.Job.IceLightningWizard:
                    break;
                case CharacterConstants.Job.IceLightningMage:
                    break;
                case CharacterConstants.Job.IceLightningArchMage:
                    break;
                case CharacterConstants.Job.Cleric:
                    break;
                case CharacterConstants.Job.Priest:
                    break;
                case CharacterConstants.Job.Bishop:
                    break;
                case CharacterConstants.Job.Hunter:
                    break;
                case CharacterConstants.Job.Ranger:
                    break;
                case CharacterConstants.Job.BowMaster:
                    break;
                case CharacterConstants.Job.CrossbowMan:
                    break;
                case CharacterConstants.Job.Sniper:
                    break;
                case CharacterConstants.Job.CrossbowMaster:
                    break;
                case CharacterConstants.Job.Assassin:
                    break;
                case CharacterConstants.Job.Hermit:
                    break;
                case CharacterConstants.Job.NightLord:
                    break;
                case CharacterConstants.Job.Bandit:
                    break;
                case CharacterConstants.Job.ChiefBandit:
                    break;
                case CharacterConstants.Job.Shadower:
                    break;
                case CharacterConstants.Job.Brawler:
                    break;
                case CharacterConstants.Job.Marauder:
                    break;
                case CharacterConstants.Job.Buccaneer:
                    break;
                case CharacterConstants.Job.Gunslinger:
                    break;
                case CharacterConstants.Job.Outlaw:
                    break;
                case CharacterConstants.Job.Corsair:
                    break;
                case CharacterConstants.Job.MapleleafBrigadier:
                    break;
                case CharacterConstants.Job.DawnWarrior2:
                    break;
                case CharacterConstants.Job.DawnWarrior3:
                    break;
                case CharacterConstants.Job.DawnWarrior4:
                    break;
                case CharacterConstants.Job.BlazeWizard2:
                    break;
                case CharacterConstants.Job.BlazeWizard3:
                    break;
                case CharacterConstants.Job.BlazeWizard4:
                    break;
                case CharacterConstants.Job.WindArcher2:
                    break;
                case CharacterConstants.Job.WindArcher3:
                    break;
                case CharacterConstants.Job.WindArcher4:
                    break;
                case CharacterConstants.Job.NightWalker2:
                    break;
                case CharacterConstants.Job.NightWalker3:
                    break;
                case CharacterConstants.Job.NightWalker4:
                    break;
                case CharacterConstants.Job.ThunderBreaker2:
                    break;
                case CharacterConstants.Job.ThunderBreaker3:
                    break;
                case CharacterConstants.Job.ThunderBreaker4:
                    break;       
                case CharacterConstants.Job.Aran2:
                    break;
                case CharacterConstants.Job.Aran3:
                    break;
                case CharacterConstants.Job.Aran4:
                    break;

                default:
                    AddMP(character, 100);
                    break;
            }
        }

        public static void GainAPOnLeveLUP(Character character)
        {
            if (CharacterJobs.IsCygnus(character) && character.Level < 70)
            {
                character.AbilityPoints += 6;
            }

            else if (CharacterJobs.IsCygnus(character) && character.Level > 70)
            {
                character.AbilityPoints += 5;
            }

            else if (CharacterJobs.IsBeginner(character) && character.Level < 8)
            {
                character.AbilityPoints += 0;

                if (character.Level < 6)
                {
                    character.Strength += 5;
                }
                else if (character.Level >= 6 && character.Level < 8)
                {
                    character.Strength += 4;
                    character.Dexterity += 1;
                }
            }

            else if (CharacterJobs.IsBeginner(character) && character.Level == 8)
            {
                character.Strength = 4;
                character.Dexterity = 4;
                character.AbilityPoints += 35;
            }

            else
            {
                character.AbilityPoints += 5;
            }
        }

        public static void GainSPOnLeveLUP(Character character)
        {
            if (CharacterJobs.IsBeginner(character))
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
            character.Update(CharacterConstants.StatisticType.Level);
            // generate randomized HP && MP bonus
            AdjustHPOnLevelUP(character);
            AdjustMPOnLevelUP(character);
            // gain stats
            // TODO: edge cases when overlevling job adv
            GainAPOnLeveLUP(character);
            GainSPOnLeveLUP(character);
            // play effect if needed
            if (PlayEffect) character.ShowRemoteUserEffect(CharacterConstants.UserEffect.LevelUp);
        }

        public static void DistributeAP(Character character, CharacterConstants.StatisticType type, short amount = 1)
        {
            switch (type)
            {
                case CharacterConstants.StatisticType.Strength:
                    character.Strength += amount;
                    break;

                case CharacterConstants.StatisticType.Dexterity:
                    character.Dexterity += amount;
                    break;

                case CharacterConstants.StatisticType.Intelligence:
                    character.Intelligence += amount;
                    break;

                case CharacterConstants.StatisticType.Luck:
                    character.Luck += amount;
                    break;

                case CharacterConstants.StatisticType.MaxHealth:
                    // TODO: Get addition based on other factors.
                    break;

                case CharacterConstants.StatisticType.MaxMana:
                    // TODO: Get addition based on other factors.
                    break;
            }
        }
    
        public static void AddAbility(Character character, CharacterConstants.StatisticType statistic, short mod, bool isReset)
        {
            short maxStat = Int16.MaxValue; // TODO: Should this be a setting?
            bool isSubtract = mod < 0;

            lock (character)
            {
                switch (statistic)
                {
                    case CharacterConstants.StatisticType.Strength:
                        if (character.Strength >= maxStat)
                        {
                            return;
                        }

                        character.Strength += mod;
                        break;

                    case CharacterConstants.StatisticType.Dexterity:
                        if (character.Dexterity >= maxStat)
                        {
                            return;
                        }

                        character.Dexterity += mod;
                        break;

                    case CharacterConstants.StatisticType.Intelligence:
                        if (character.Intelligence >= maxStat)
                        {
                            return;
                        }

                        character.Intelligence += mod;
                        break;

                    case CharacterConstants.StatisticType.Luck:
                        if (character.Luck >= maxStat)
                        {
                            return;
                        }

                        character.Luck += mod;
                        break;

                    case CharacterConstants.StatisticType.MaxHealth:
                    case CharacterConstants.StatisticType.MaxMana:
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


        //TODO: hp/mp modification bugs out UI bars, add multiple stats, some kind of message to sideBar/chat
        public static void giveStat(Character player, CharacterConstants.StatisticType stat, short quantity)
        {
            switch (stat)
            {
                case CharacterConstants.StatisticType.Strength:
                    int totalStrenght = player.Strength + quantity;

                    if (totalStrenght < Int16.MaxValue)
                    {
                        player.Strength += quantity;
                        player.Update(CharacterConstants.StatisticType.Strength);
                        break;
                    }

                    else
                    {
                        player.Strength = Int16.MaxValue;
                        player.Update(CharacterConstants.StatisticType.Strength);
                        break;
                    }

                case CharacterConstants.StatisticType.Dexterity:
                    int totalDexterity = player.Dexterity + quantity;

                    if (totalDexterity < Int16.MaxValue)
                    {
                        player.Dexterity += quantity;
                        player.Update(CharacterConstants.StatisticType.Dexterity);
                        break;
                    }

                    else
                    {
                        player.Dexterity = Int16.MaxValue;
                        player.Update(CharacterConstants.StatisticType.Dexterity);
                        break;
                    }

                case CharacterConstants.StatisticType.Intelligence:
                    int totalIntelligence = player.Intelligence + quantity;

                    if (totalIntelligence < Int16.MaxValue)
                    {
                        player.Intelligence += quantity;
                        player.Update(CharacterConstants.StatisticType.Intelligence);
                        break;
                    }

                    else
                    {
                        player.Intelligence = Int16.MaxValue;
                        player.Update(CharacterConstants.StatisticType.Intelligence);
                        break;
                    }

                case CharacterConstants.StatisticType.Luck:
                    int totalLuck = player.Luck + quantity;
                    

                    if (totalLuck < Int16.MaxValue)
                    {
                        player.Luck += quantity;
                        player.Update(CharacterConstants.StatisticType.Luck);
                        break;
                    }

                    else
                    {
                        player.Luck = Int16.MaxValue;
                        player.Update(CharacterConstants.StatisticType.Luck);
                        break;
                    }

                case CharacterConstants.StatisticType.Health:
                    int totalHealth = player.Health + quantity;

                    if (totalHealth < Int16.MaxValue)
                    {
                        player.Health += quantity;
                        player.Update(CharacterConstants.StatisticType.Health);
                        break;
                    }

                    else
                    {
                        player.Health = Int16.MaxValue;
                        player.Update(CharacterConstants.StatisticType.Health);
                        break;
                    }

                case CharacterConstants.StatisticType.MaxHealth:
                    int totalMaxHealth = player.MaxHealth + quantity;

                    if (totalMaxHealth < Int16.MaxValue)
                    {
                        player.MaxHealth += quantity;
                        player.Update(CharacterConstants.StatisticType.MaxHealth);
                        break;
                    }

                    else
                    {
                        player.MaxHealth = Int16.MaxValue;
                        player.Update(CharacterConstants.StatisticType.MaxHealth);
                        break;
                    }

                case CharacterConstants.StatisticType.Mana:
                    int totalMana = player.Mana + quantity;

                    if (totalMana < Int16.MaxValue)
                    {
                        player.Mana += quantity;
                        player.Update(CharacterConstants.StatisticType.Mana);
                        break;
                    }

                    else
                    {
                        player.Mana = Int16.MaxValue;
                        player.Update(CharacterConstants.StatisticType.Mana);
                        break;
                    }

                case CharacterConstants.StatisticType.MaxMana:
                    int totalMaxMana = player.MaxMana + quantity;

                    if (totalMaxMana < Int16.MaxValue)
                    {
                        player.MaxMana += quantity;
                        player.Update(CharacterConstants.StatisticType.MaxMana);
                        break;
                    }

                    else
                    {
                        player.MaxMana = Int16.MaxValue;
                        player.Update(CharacterConstants.StatisticType.MaxMana);
                        break;
                    }

                case CharacterConstants.StatisticType.AbilityPoints:
                    int totalAbilityPoints = player.AbilityPoints + quantity;

                    if (totalAbilityPoints < Int16.MaxValue)
                    {
                        player.AbilityPoints += quantity;
                        player.Update(CharacterConstants.StatisticType.AbilityPoints);
                        break;
                    }

                    else
                    {
                        player.AbilityPoints = Int16.MaxValue;
                        player.Update(CharacterConstants.StatisticType.AbilityPoints);
                        break;
                    }

                case CharacterConstants.StatisticType.SkillPoints:
                    int totalSkillPoints = player.SkillPoints + quantity;

                    if (totalSkillPoints < Int16.MaxValue)
                    {
                        player.SkillPoints += quantity;
                        player.Update(CharacterConstants.StatisticType.SkillPoints);
                        break;
                    }
                    else
                    {
                        player.SkillPoints = Int16.MaxValue;
                        player.Update(CharacterConstants.StatisticType.SkillPoints);
                        break;
                    }

                case CharacterConstants.StatisticType.Skin: break;
                case CharacterConstants.StatisticType.Face: break;
                case CharacterConstants.StatisticType.Hair: break;
                case CharacterConstants.StatisticType.Level: break;
                case CharacterConstants.StatisticType.Job: break;
                case CharacterConstants.StatisticType.Experience: break;
                case CharacterConstants.StatisticType.Fame: break;
                case CharacterConstants.StatisticType.Mesos: break;
                case CharacterConstants.StatisticType.Pet: break;
                case CharacterConstants.StatisticType.GachaponExperience: break;

                default: throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }

        protected override int GetKeyForItem(CharacterStats item)
        {
            throw new NotImplementedException();
        }
    }
}