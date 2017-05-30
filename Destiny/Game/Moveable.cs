using Destiny.Core.IO;

namespace Destiny.Game
{
    // TODO: Find a proper name for this class.
    public abstract class Moveable
    {
        public byte Stance { get; set; }
        public short Foothold { get; set; }
        public Point Position { get; set; }

        protected Moveable()
        {
            this.Stance = 0;
            this.Foothold = 0;
            this.Position = new Point(0, 0);
        }

        public void Decode(InPacket iPacket)
        {

        }
    }
}
