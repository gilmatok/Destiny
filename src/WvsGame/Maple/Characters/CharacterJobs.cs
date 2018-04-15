using Destiny.Constants;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterJobs
    {
        public Character Player { get; private set; }
        public CharacterConstants.Job Job;

        #region AdventurerJobs
        public static bool IsAdventurerBeginner(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            switch (currentJob)
            {
                case CharacterConstants.Job.Beginner:
                    return true;

                default: return false;
            }
        }

        public static bool IsAdventurerFirstJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            switch (currentJob)
            {
                case CharacterConstants.Job.Warrior:
                    return true;
                case CharacterConstants.Job.Magician:
                    return true;
                case CharacterConstants.Job.Bowman:
                    return true;
                case CharacterConstants.Job.Thief:
                    return true;
                case CharacterConstants.Job.Pirate:
                    return true;

                default: return false;
            }
        }    

        public static bool IsAdventurerSecondJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            switch (currentJob)
            {
                // warriors
                case CharacterConstants.Job.Page:
                    return true;
                case CharacterConstants.Job.Fighter:
                    return true;
                case CharacterConstants.Job.Spearman:
                    return true;
                // mages
                case CharacterConstants.Job.Cleric:
                    return true;
                case CharacterConstants.Job.IceLightningWizard:
                    return true;
                case CharacterConstants.Job.FirePoisonWizard:
                    return true;
                // thiefs
                case CharacterConstants.Job.Assassin:
                    return true;
                case CharacterConstants.Job.Bandit:
                    return true;
                // archers
                case CharacterConstants.Job.Hunter:
                    return true;
                case CharacterConstants.Job.CrossbowMan:
                    return true;
                // pirates
                case CharacterConstants.Job.Gunslinger:
                    return true;
                case CharacterConstants.Job.Brawler:
                    return true;

                default: return false;
            }
        }

        public static bool IsAdventurerThirdJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            switch (currentJob)
            {
                case CharacterConstants.Job.Warrior:
                    return true;
                case CharacterConstants.Job.Magician:
                    return true;
                case CharacterConstants.Job.Bowman:
                    return true;
                case CharacterConstants.Job.Thief:
                    return true;
                case CharacterConstants.Job.Pirate:
                    return true;

                default: return false;
            }
        }

        public static bool IsAdventurerFourthJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            switch (currentJob)
            {
                case CharacterConstants.Job.Warrior:
                    return true;
                case CharacterConstants.Job.Magician:
                    return true;
                case CharacterConstants.Job.Bowman:
                    return true;
                case CharacterConstants.Job.Thief:
                    return true;
                case CharacterConstants.Job.Pirate:
                    return true;

                default: return false;
            }
        }

        public static bool IsAdventurer(Character Player)
        {
            return IsAdventurerBeginner(Player) || IsAdventurerFirstJob(Player) || IsAdventurerSecondJob(Player)
                   || IsAdventurerThirdJob(Player) || IsAdventurerFourthJob(Player);
        }
        #endregion

        #region CygnusKnightJobs
        public static bool IsCygnusBeginner(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            switch (currentJob)
            {
                case CharacterConstants.Job.Noblesse:
                    return true;

                default: return false;
            }
        }

        public static bool IsCygnusFirstJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            switch (currentJob)
            {
                case CharacterConstants.Job.DawnWarrior1:
                    return true;
                case CharacterConstants.Job.BlazeWizard1:
                    return true;
                case CharacterConstants.Job.WindArcher1:
                    return true;
                case CharacterConstants.Job.NightWalker1:
                    return true;
                case CharacterConstants.Job.ThunderBreaker1:
                    return true;

                default: return false;
            }
        }

        public static bool IsCygnusSecondJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            switch (currentJob)
            {
                case CharacterConstants.Job.DawnWarrior2:
                    return true;
                case CharacterConstants.Job.BlazeWizard2:
                    return true;
                case CharacterConstants.Job.WindArcher2:
                    return true;
                case CharacterConstants.Job.NightWalker2:
                    return true;
                case CharacterConstants.Job.ThunderBreaker2:
                    return true;

                default: return false;
            }
        }

        public static bool IsCygnusThirdJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            switch (currentJob)
            {
                case CharacterConstants.Job.DawnWarrior3:
                    return true;
                case CharacterConstants.Job.BlazeWizard3:
                    return true;
                case CharacterConstants.Job.WindArcher3:
                    return true;
                case CharacterConstants.Job.NightWalker3:
                    return true;
                case CharacterConstants.Job.ThunderBreaker3:
                    return true;

                default: return false;
            }
        }

        public static bool IsCygnusFourthJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            switch (currentJob)
            {
                case CharacterConstants.Job.DawnWarrior4:
                    return true;
                case CharacterConstants.Job.BlazeWizard4:
                    return true;
                case CharacterConstants.Job.WindArcher4:
                    return true;
                case CharacterConstants.Job.NightWalker4:
                    return true;
                case CharacterConstants.Job.ThunderBreaker4:
                    return true;

                default: return false;
            }
        }

        public static bool IsCygnus(Character Player)
        {
            return IsCygnusBeginner(Player) || IsCygnusFirstJob(Player) || IsCygnusSecondJob(Player)
                || IsCygnusThirdJob(Player) || IsCygnusFourthJob(Player);
        }
        #endregion

        #region AranJobs
        public static bool IsAranBeginner(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            return currentJob == CharacterConstants.Job.Aran;
        }

        public static bool IsAranFirstJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            return currentJob == CharacterConstants.Job.Aran1;
        }

        public static bool IsAranSecondJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            return currentJob == CharacterConstants.Job.Aran2;
        }

        public static bool IsAranThirdJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            return currentJob == CharacterConstants.Job.Aran3;
        }

        public static bool IsAranFourthJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            return currentJob == CharacterConstants.Job.Aran4;
        }

        public static bool IsAran(Character Player)
        {
            return IsAranBeginner(Player) || IsAranFirstJob(Player) || IsAranSecondJob(Player)
                   || IsAranThirdJob(Player) || IsAranFourthJob(Player);
        }
        #endregion

        #region UniqueJobs
        public static bool IsMapleleafBrigadierJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            return currentJob == CharacterConstants.Job.MapleleafBrigadier;
        }

        public static bool IsGMJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            return currentJob == CharacterConstants.Job.GM;
        }

        public static bool IsSuperGMJob(Character Player)
        {
            CharacterConstants.Job currentJob = Player.Job;

            return currentJob == CharacterConstants.Job.SuperGM;
        }

        public static bool IsUniqueJob(Character Player)
        {
            return IsMapleleafBrigadierJob(Player) || IsGMJob(Player) || IsSuperGMJob(Player);
        }
        #endregion

        public static bool IsBeginner(Character Player)
        {
            return IsAdventurerBeginner(Player) || IsCygnusBeginner(Player) || IsAranBeginner(Player);
        }

        public static bool IsFirstJob(Character Player)
        {
            return IsAdventurerFirstJob(Player) || IsCygnusFirstJob(Player) || IsAranFirstJob(Player);
        }

        public static bool IsSecondJob(Character Player)
        {
            return IsAdventurerSecondJob(Player) || IsCygnusSecondJob(Player) || IsAranSecondJob(Player);
        }

        public static bool IsThirdJob(Character Player)
        {
            return IsAdventurerThirdJob(Player) || IsCygnusThirdJob(Player) || IsAranThirdJob(Player);
        }

        public static bool IsFourthJob(Character Player)
        {
            return IsAdventurerFourthJob(Player) || IsCygnusFourthJob(Player) || IsAranFourthJob(Player);
        }
    }
}