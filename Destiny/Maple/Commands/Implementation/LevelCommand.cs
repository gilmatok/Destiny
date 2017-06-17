using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands
{
    public sealed class LevelCommand : Command
    {
        public override string Name => "level";
        public override string Parameters => "level";
        public override GmLevel RequiredLevel => GmLevel.Gm;

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 1)
                this.ShowSyntax(caller);
            else
                caller.Stats.Level = byte.Parse(args[0]);
        }
    }
}
