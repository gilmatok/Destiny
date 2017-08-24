using Destiny.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Maps.Portals
{
    public sealed class market19 : Portal
    {
        public market19(Datum datum) : base(datum) { }

        public override void Enter(Character character)
        {
            character.Variables.Add(new Variable("fm_origin", character.Map.MapleID));
            character.Variables.Add(new Variable("fm_origin_portal", this.Label));

            this.PlaySoundEffect(character);

            character.ChangeMap(910000000, "out00");
        }
    }
}
