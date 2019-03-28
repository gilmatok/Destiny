using Destiny.Maple.Characters;
using System;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class ExpCommand : Command
    {
        public override string Name
        {
            get
            {
                return "exp";
            }
        }

        public override string Parameters
        {
            get
            {
                return "{ amount }";
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
            if (args.Length != 1)
            {
                this.ShowSyntax(caller);
            }
            else
            {
				if (int.TryParse(args[0], out int expGiven))
				{
					Experience.giveExp(caller, expGiven);
				}
				else
				{
					caller.Notify("[Command] Invalid amount provided.");
				}
			}
        }

    }
}