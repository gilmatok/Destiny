using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Maps.Portals
{
    public sealed class tutoChatNPC : Portal
    {
        public tutoChatNPC(Datum datum) : base(datum) { }

        public override void Enter(Character character)
        {
            if (false)
            {
                // TODO: Check if the account has any characters over level 30.

                character.Converse(2007);
            }
        }
    }
}
