using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Maps.Portals
{
    public sealed class glTutoMsg0 : Portal
    {
        public glTutoMsg0(Datum datum) : base(datum) { }

        public override void Enter(Character character)
        {
            this.ShowBalloonMessage(character, "Once you leave this area you won't be able to return.", 150, 5);
        }
    }
}
