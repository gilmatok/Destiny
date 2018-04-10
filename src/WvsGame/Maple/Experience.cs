using System;
using Destiny.Maple.Characters;
using Destiny.Network;

namespace Destiny.Maple
{
    public sealed class Experience
    {
        public int Amount { get; private set; }

        public Experience(int amount)
            : base()
        {
            this.Amount = amount;
        }

        public static void giveExp(Character character, int exp)
        {
            long myPlusGivenExp = (long)character.Experience + (long)exp;

            if (myPlusGivenExp > Int32.MaxValue)
            {
                character.Experience = Int32.MaxValue;
            }
            else
            {
                character.Experience += exp;
                Packet ShowGivenExp = GetShowExpGainPacket(true, exp, false, 0, 0);
                character.Client.Send(ShowGivenExp);
            }
        }

        public static Packet GetShowExpGainPacket(bool white, int ammount, bool inChat, int partyBonus, int equipBonus)
        {
            return Character.GetShowSidebarInfoPacket(MessageType.IncreaseEXP, white, 0, ammount, inChat, partyBonus, equipBonus);
        }

    }
}