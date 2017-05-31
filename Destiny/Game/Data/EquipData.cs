using System.IO;

namespace Destiny.Game.Data
{
    public sealed class EquipData : ItemData
    {
        public byte Slots { get; set; }
        public short Strength { get; set; }
        public short Dexterity { get; set; }
        public short Intelligence { get; set; }
        public short Luck { get; set; }
        public short Health { get; set; }
        public short Mana { get; set; }
        public short WeaponAttack { get; set; }
        public short MagicAttack { get; set; }
        public short WeaponDefense { get; set; }
        public short MagicDefense { get; set; }
        public short Accuracy { get; set; }
        public short Avoidability { get; set; }
        public short Hands { get; set; }
        public short Speed { get; set; }
        public short Jump { get; set; }

        public override void Load(BinaryReader reader)
        {
            base.Load(reader);

            this.Slots = reader.ReadByte();
            this.Strength = reader.ReadInt16();
            this.Dexterity = reader.ReadInt16();
            this.Intelligence = reader.ReadInt16();
            this.Luck = reader.ReadInt16();
            this.Health = reader.ReadInt16();
            this.Mana = reader.ReadInt16();
            this.WeaponAttack = reader.ReadInt16();
            this.MagicAttack = reader.ReadInt16();
            this.WeaponDefense= reader.ReadInt16();
            this.MagicDefense= reader.ReadInt16();
            this.Accuracy = reader.ReadInt16();
            this.Avoidability = reader.ReadInt16();
            this.Hands = reader.ReadInt16();
            this.Speed = reader.ReadInt16();
            this.Jump = reader.ReadInt16();
        }

        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);

            writer.Write(this.Slots);
            writer.Write(this.Strength);
            writer.Write(this.Dexterity);
            writer.Write(this.Intelligence);
            writer.Write(this.Luck);
            writer.Write(this.Health);
            writer.Write(this.Mana);
            writer.Write(this.WeaponAttack);
            writer.Write(this.MagicAttack);
            writer.Write(this.WeaponDefense);
            writer.Write(this.MagicDefense);
            writer.Write(this.Accuracy);
            writer.Write(this.Avoidability);
            writer.Write(this.Hands);
            writer.Write(this.Speed);
            writer.Write(this.Jump);
        }
    }
}
