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

        public static Packet GetShowExpGainPacket(bool white, int ammount, bool inChat, int partyBonus, int equipBonus)
        {
            return Character.GetShowSidebarInfoPacket(MessageType.IncreaseEXP, white, 0, ammount, inChat, partyBonus, equipBonus);
        }

    }
}