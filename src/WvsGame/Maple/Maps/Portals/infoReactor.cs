using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Maps.Portals
{
    public sealed class infoReactor : Portal
    {
        public infoReactor(Datum datum) : base(datum) { }

        public override void Enter(Character character)
        {
            if (character.Quests.Completed.ContainsKey(1008))
            {
                this.ShowTutorialMessage(character, "UI/tutorial.img/22");
            }
            else if (character.Quests.Completed.ContainsKey(1020))
            {
                this.ShowTutorialMessage(character, "UI/tutorial.img/27");
            }
        }
    }
}
