namespace Destiny.Game
{
    public sealed class Map
    {
        public const int INVALID_MAP_ID = 999999999;

        public int Identifier { get; private set; }

        public Map(int identifier)
        {
            this.Identifier = identifier;
        }
    }
}
