using System.IO;

namespace Destiny.Game
{
    public sealed class Npc
    {
        public int Identifier { get; private set; }
        public NpcFlags Flags { get; private set; }
        public ushort StorageCost { get; private set; }
        
        public Npc(BinaryReader reader)
        {
            this.Identifier = reader.ReadInt32();
            this.Flags = (NpcFlags)reader.ReadByte();
            this.StorageCost = reader.ReadUInt16();

            int shopsCount = reader.ReadInt32();

            while (shopsCount-- > 0)
            {
                reader.ReadInt32();
                reader.ReadByte();

                int shopItemCount = reader.ReadInt32();

                while (shopItemCount-- > 0)
                {
                    reader.ReadInt32();
                    reader.ReadUInt16();
                    reader.ReadInt32();
                    reader.ReadSingle();
                }
            }
        }
    }
}
