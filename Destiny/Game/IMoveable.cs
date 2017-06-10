namespace Destiny.Game
{
    public interface IMoveable
    {
        byte Stance { get; set; }
        short Foothold { get; set; }
        Point Position { get; set; }
    }
}
