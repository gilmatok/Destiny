using System;

namespace Destiny.Game
{
    public sealed class Map
    {
        public const int INVALID_MAP_ID = 999999999;

        public int MapleID { get; private set; }
        public MapCharacters Characters { get; private set; }
        public MapNpcs Npcs { get; private set; }

        public Map(int identifier)
        {
            this.MapleID = identifier;
            this.Characters = new MapCharacters(this);
            this.Npcs = new MapNpcs(this);
        }

        private int mNpcObjectIDs = 0;
        private int mMobObjectIDs = 0;
        private int mReactorObjectIDs = 0;

        public int AssignObjectID(MapObjectType type)
        {
            switch (type)
            {
                case MapObjectType.Npc: return ++mNpcObjectIDs;
                case MapObjectType.Mob: return ++mMobObjectIDs;
                case MapObjectType.Reactor: return ++mReactorObjectIDs;
                default: throw new ArgumentException(type.ToString());
            }
        }
    }
}
