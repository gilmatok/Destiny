using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class KickCommand : Command
    {
        public override string Name
        {
            get
            {
                return "kick";
            }
        }

        public override string Parameters
        {
            get
            {
                return "[character]";
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
            if (args.Length == 0)
            {
                this.ShowSyntax(caller);
            }
            else
            {
                string name = args[0];
                Character target = null;// caller.Client.World.GetCharacter(name);

                if (target == null)
                {
                    caller.Notify($"[Command] Character '{name}' could not be found.");

                    return;
                }

                target.Client.Stop();
            }
        }
    }
}
