using System.IO;

namespace Destiny.Game
{
    public sealed class Map
    {
        public const int INVALID_MAP_ID = 999999999;

        public int Identifier { get; private set; }
        public EMapFlags Flags { get; private set; }
        public string ShuffleName { get; private set; }
        public string Music { get; private set; }
        public byte MinLevelLimit { get; private set; }
        public ushort TimeLimit { get; private set; }
        public byte RegenRate { get; private set; }
        public float Traction { get; private set; }
        public short LeftTopX { get; private set; }
        public short LeftTopY { get; private set; }
        public short RightBottomX { get; private set; }
        public short RightBottomY { get; private set; }
        public int ReturnMapIdentifier { get; private set; }
        public int ForcedReturnMapIdentifier { get; private set; }
        public EMapFieldType FieldTypes { get; private set; }
        public EMapFieldLimit FieldLimits { get; private set; }
        public byte DecreaseHP { get; private set; }
        public ushort DamagePerSecond { get; private set; }
        public int ProtectItemIdentifier { get; private set; }
        public float MobRate { get; private set; }
        public int LinkIdentifier { get; private set; }

        public Map(BinaryReader reader)
        {
            this.Identifier = reader.ReadInt32();
            this.Flags = (EMapFlags)reader.ReadUInt16();
            this.ShuffleName = reader.ReadString();
            this.Music = reader.ReadString();
            this.MinLevelLimit = reader.ReadByte();
            this.TimeLimit = reader.ReadUInt16();
            this.RegenRate = reader.ReadByte();
            this.Traction = reader.ReadSingle();
            this.LeftTopX = reader.ReadInt16();
            this.LeftTopY = reader.ReadInt16();
            this.RightBottomX = reader.ReadInt16();
            this.RightBottomY = reader.ReadInt16();
            this.ReturnMapIdentifier = reader.ReadInt32();
            this.ForcedReturnMapIdentifier = reader.ReadInt32();
            this.FieldTypes = (EMapFieldType)reader.ReadUInt16();
            this.FieldLimits = (EMapFieldLimit)reader.ReadUInt32();
            this.DecreaseHP = reader.ReadByte();
            this.DamagePerSecond = reader.ReadUInt16();
            this.ProtectItemIdentifier = reader.ReadInt32();
            this.MobRate = reader.ReadSingle();
            this.LinkIdentifier = reader.ReadInt32();

            int footholdsCount = reader.ReadInt32();

            while (footholdsCount-- > 0)
            {
                reader.ReadUInt16();
                reader.ReadByte();
                reader.ReadUInt16();
                reader.ReadUInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
            }

            int npcsCount = reader.ReadInt32();

            while (npcsCount-- > 0)
            {
                reader.ReadInt32();
                reader.ReadByte();
                reader.ReadUInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
            }

            int reactorsCount = reader.ReadInt32();

            while (reactorsCount-- > 0)
            {
                reader.ReadInt32();
                reader.ReadByte();
                reader.ReadUInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt32();
                reader.ReadString();
            }

            int mobsCount = reader.ReadInt32();

            while (mobsCount-- > 0)
            {
                reader.ReadInt32();
                reader.ReadByte();
                reader.ReadUInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt32();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadString();
            }

            int portalsCount = reader.ReadInt32();

            while (portalsCount-- > 0)
            {
                reader.ReadByte();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadString();
                reader.ReadInt32();
                reader.ReadString();
                reader.ReadString();
            }

            int seatsCount = reader.ReadInt32();

            while (seatsCount-- > 0)
            {
                reader.ReadInt16();
                reader.ReadInt16();
            }
        }
    }
}
