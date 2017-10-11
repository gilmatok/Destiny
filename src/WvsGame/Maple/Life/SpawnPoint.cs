using Destiny.Data;

namespace Destiny.Maple.Life
{
    public sealed class SpawnPoint : LifeObject
    {
        public bool IsMob { get; private set; }

        public SpawnPoint(Datum datum, bool isMob)
            : base(datum)
        {
            this.IsMob = isMob;
        }

        public void Spawn()
        {
            if (this.IsMob)
            {
                this.Map.Mobs.Add(new Mob(this));
            }
            else
            {
                this.Map.Reactors.Add(new Reactor(this));
            }
        }
    }
}
