using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Maps.Portals
{
    public sealed class infoMinimap : Portal
    {
        public infoMinimap(Datum datum) : base(datum) { }

        public override void Enter(Character character)
        {
            if (character.Quests.Started.ContainsKey(1031))
            {
                this.ShowTutorialMessage(character, "UI/tutorial.img/25");
            }
        }
    }
}
