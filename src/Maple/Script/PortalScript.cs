using Destiny.Maple.Characters;
using Destiny.Maple.Maps;

namespace Destiny.Maple.Script
{
    public sealed class PortalScript : ScriptBase
    {
        private Portal mPortal;

        public PortalScript(Portal portal, Character character)
            : base(portal.ScriptPath, character)
        {
            mPortal = portal;
        }
    }
}
