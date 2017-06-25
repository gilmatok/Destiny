using Destiny.Data;

namespace Destiny.Maple.Life
{
    public sealed class SpawnPoint : LifeObject
    {
        public SpawnPoint(Datum datum) : base(datum) { }

        public void Spawn()
        {
            this.Map.Mobs.Add(new Mob(this));
        }
    }
}
