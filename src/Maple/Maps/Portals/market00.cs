using Destiny.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Maps.Portals
{
    public sealed class market00 : Portal
    {
        public market00(Datum datum) : base(datum) { }

        public override void Enter(Character character)
        {
            // TODO: Get the last map/portal.
            // If it fails for any reason, use default (Henesys Market).

            int map = 100000100;
            string portal = "market00";

            this.PlaySoundEffect(character);

            character.ChangeMap(map, portal);
        }
    }
}
