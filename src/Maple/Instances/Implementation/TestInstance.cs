using Destiny.Maple.Characters;
using Destiny.Server;

namespace Destiny.Maple.Instances.Implementation
{
    public sealed class TestInstance : Instance
    {
        public override string Name
        {
            get
            {
                return "Test";
            }
        }

        public TestInstance(ChannelServer channel) : base(channel) { }

        public override void Start()
        {
            this.AddMap(0);

            Log.Inform("Test Instance is initialized!");
        }

        public override void TimerEnd(string label)
        {
            if (label == "Main")
            {
                if (this.Characters.Count > 0)
                {
                    this.WarpCharacters(100000000);

                    this.RemoveCharacters();
                }
            }
        }

        public override void CharacterDeath(Character character)
        {
            character.Notify("Boo! You died!");
        }
    }
}
