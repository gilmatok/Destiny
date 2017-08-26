using Destiny.Maple.Characters;
using Destiny.Maple.Life;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class ZakumCommand : Command
    {
        public override string Name
        {
            get
            {
                return "zakum";
            }
        }

        public override string Parameters
        {
            get
            {
                return string.Empty;
            }
        }

        public override bool IsRestricted
        {
            get
            {
                return true;
            }
        }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 0)
            {
                this.ShowSyntax(caller);
            }
            else
            {
                caller.Map.Mobs.Add(new Mob(8800000, caller.Position));

                for (int i = 0; i < 7; i++)
                {
                    caller.Map.Mobs.Add(new Mob(8800003 + i, caller.Position));
                }
            }
        }
    }
}
