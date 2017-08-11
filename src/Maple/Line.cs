namespace Destiny.Maple
{
    public sealed class Line
    {
        public Point Point1 { get; private set; }
        public Point Point2 { get; private set; }

        public Line(Point point1, Point point2)
        {
            this.Point1 = point1;
            this.Point2 = point2;
        }
    }
}
