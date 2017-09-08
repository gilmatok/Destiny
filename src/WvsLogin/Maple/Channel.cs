using Destiny.Network;

namespace Destiny.Maple
{
    public sealed class Channel
    {
        public byte ID { get; set; }
        public ushort Port { get; set; }
        public int Population { get; set; }

        public Channel(Packet inPacket)
        {
            this.ID = inPacket.ReadByte();
            this.Port = inPacket.ReadUShort();
            this.Population = inPacket.ReadInt();
        }
    }
}
