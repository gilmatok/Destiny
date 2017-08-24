using Destiny.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Maps.Portals
{
    public sealed class market00 : Portal
    {
        public market00(Datum datum) : base(datum) { }

        public override void Enter(Character character)
        {
            int map;
            string portal;

            if (character.Variables.Contains("fm_origin"))
            {
                map = int.Parse(character.Variables["fm_origin"].Value);
                portal = character.Variables["fm_origin_portal"].Value;
            }
            else
            {
                map = 100000100;
                portal = "market00";
            }

            this.PlaySoundEffect(character);

            character.ChangeMap(map, portal);

            character.Variables.Remove("fm_origin");
            character.Variables.Remove("fm_origin_portal");
        }
    }
}
