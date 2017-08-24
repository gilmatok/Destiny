using Destiny.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Maps.Portals
{
    public sealed class market01 : Portal
    {
        public market01(Datum datum) : base(datum) { }

        public override void Enter(Character character)
        {
            // TODO: Keep track of the last map/portal.

            this.PlaySoundEffect(character);

            character.ChangeMap(910000000, "out00");
        }
    }
}
