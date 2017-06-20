namespace Destiny.Maple
{
    public interface IMoveable
    {
        byte Stance { get; set; }
        short Foothold { get; set; }
        Point Position { get; set; }
    }
}
