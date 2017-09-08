using Destiny.Maple.Characters;
using Destiny.Maple.Life;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class HorntailCommand : Command
    {
        public override string Name
        {
            get
            {
                return "horntail";
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
                Mob mob = new Mob(8810026) // TODO: Get from strings 'Summon Horntail' so we don't have to deal with
                {
                    Position = caller.Position
                };

                caller.Map.Mobs.Add(mob);
                caller.Map.Mobs.Remove(mob);
            }
        }
    }
}
