namespace Destiny.Game
{
    public class Rectangle
    {
        public Point LT { get; set; }
        public Point RB { get; set; }

        public Rectangle(Point lt, Point rb)
        {
            this.LT = lt;
            this.RB = rb;
        }
    }
}
