using Destiny.Core.IO;
using Destiny.Utility;

namespace Destiny.Game
{
    public sealed class Equip : Item
    {
        public byte Slots { get; private set; }
        public byte Scrolls { get; private set; }
        public short Strength { get; private set; }
        public short Dexterity { get; private set; }
        public short Intelligence { get; private set; }
        public short Luck { get; private set; }
        public short Health { get; private set; }
        public short Mana { get; private set; }
        public short WeaponAttack { get; private set; }
        public short MagicAttack { get; private set; }
        public short WeaponDefense { get; private set; }
        public short MagicDefense { get; private set; }
        public short Accuracy { get; private set; }
        public short Avoidability { get; private set; }
        public short Hands { get; private set; }
        public short Speed { get; private set; }
        public short Jump { get; private set; }

        public Equip(DatabaseQuery query)
            : base(query)
        {
            this.Slots = query.GetByte("slots");
            this.Scrolls = query.GetByte("scrolls");
            this.Strength = query.GetShort("strength");
            this.Dexterity = query.GetShort("dexterity");
            this.Intelligence = query.GetShort("intelligence");
            this.Luck = query.GetShort("luck");
            this.Health = query.GetShort("health");
            this.Mana = query.GetShort("mana");
            this.WeaponAttack = query.GetShort("weapon_attack");
            this.MagicAttack = query.GetShort("magic_attack");
            this.WeaponDefense = query.GetShort("weapon_defense");
            this.MagicDefense = query.GetShort("magic_defense");
            this.Accuracy = query.GetShort("accuracy");
            this.Avoidability = query.GetShort("avoidability");
            this.Hands = query.GetShort("hands");
            this.Speed = query.GetShort("speed");
            this.Jump = query.GetShort("jump");
        }

        public override void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteByte(1)
                .WriteInt(this.Identifier)
                .WriteBool(false)
                .WriteLong() // TODO: Expiration.
                .WriteByte(this.Slots)
                .WriteByte(this.Scrolls)
                .WriteShort(this.Strength)
                .WriteShort(this.Dexterity)
                .WriteShort(this.Intelligence)
                .WriteShort(this.Luck)
                .WriteShort(this.Health)
                .WriteShort(this.Mana)
                .WriteShort(this.WeaponAttack)
                .WriteShort(this.MagicAttack)
                .WriteShort(this.WeaponDefense)
                .WriteShort(this.MagicDefense)
                .WriteShort(this.Accuracy)
                .WriteShort(this.Avoidability)
                .WriteShort(this.Hands)
                .WriteShort(this.Speed)
                .WriteShort(this.Jump)
                .WriteString(string.Empty) // NOTE: Creator.
                .WriteShort() // NOTE: Flags.
                .WriteByte()
                .WriteByte()
                .WriteShort()
                .WriteShort()
                .WriteInt()
                .WriteLong()
                .WriteLong()
                .WriteInt(-1);
        }
    }
}
