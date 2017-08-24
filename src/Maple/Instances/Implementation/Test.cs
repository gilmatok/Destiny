using Destiny.Server;

namespace Destiny.Maple.Instances.Implementation
{
    public sealed class Test : Instance
    {
        public override string Name
        {
            get
            {
                return "Test";
            }
        }

        public override bool ShowTimer
        {
            get
            {
                return true;
            }
        }

        public Test(ChannelServer channel, int time)
            : base(channel, time)
        {
            this.AddMap(0);
        }

        public override void TimerEnd(string label)
        {
            if (label == "Main")
            {
                this.WarpCharacters(180000000);

                this.RemoveCharacters();
            }
        }
    }
}
