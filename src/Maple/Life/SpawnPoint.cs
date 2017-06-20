using Destiny.Utility;

namespace Destiny.Maple.Life
{
    public sealed class SpawnPoint : LifeObject
    {
        public SpawnPoint(DatabaseQuery query) : base(query) { }

        public void Spawn()
        {
            this.Map.Mobs.Add(new Mob(this));
        }
    }
}
