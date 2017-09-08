using System;

namespace Destiny.Maple
{
    public class Point
    {
        public short X { get; set; }
        public short Y { get; set; }

        public Point(short x, short y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point(int x, int y)
        {
            this.X = (short)x;
            this.Y = (short)y;
        }

        public double DistanceFrom(Point point)
        {
            return Math.Sqrt(Math.Pow(this.X - point.X, 2) + Math.Pow(this.Y - point.Y, 2));
        }

        public bool IsInRectangle(Rectangle rectangle)
        {
            return this.X >= rectangle.LT.X &&
                this.Y >= rectangle.LT.Y &&
                this.X <= rectangle.RB.X &&
                this.Y <= rectangle.RB.Y;
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }
    }
}
