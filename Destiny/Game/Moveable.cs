using Destiny.Core.IO;

namespace Destiny.Game
{
    // TODO: Find a proper name for this class.
    public abstract class Moveable
    {
        public Point Position { get; set; }
        public short Foothold { get; set; }
        public byte Stance { get; set; }

        protected Moveable()
        {
            this.Position = new Point(0, 0);
            this.Foothold = 0;
            this.Stance = 0;
        }

        public void Decode(InPacket iPacket)
        {

        }
    }
}
