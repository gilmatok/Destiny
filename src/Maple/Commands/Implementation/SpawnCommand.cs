using System;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Maple.Life;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class SpawnCommand : Command
    {
        public override string Name
        {
            get
            {
                return "spawn";
            }
        }

        public override string Parameters
        {
            get
            {
                return "{ id | exact name } [ amount ] ";
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
            if (args.Length < 1)
            {
                this.ShowSyntax(caller);
            }
            else
            {
                int amount = 0;
                bool isAmountSpecified;

                if (args.Length > 1)
                {
                    isAmountSpecified = int.TryParse(args[args.Length - 1], out amount);
                }
                else
                {
                    isAmountSpecified = false;
                }

                if (amount < 1)
                {
                    amount = 1;
                }

                int mobId = -1;

                try
                {
                    mobId = int.Parse(args[0]);
                }
                catch (FormatException)
                {
                    // TODO: Fetch from strings.
                }

                if (DataProvider.Mobs.Contains(mobId))
                {
                    for (int i = 0; i < amount; i++)
                    {
                        caller.Map.Mobs.Add(new Mob(mobId, caller.Position));
                    }
                }
                else
                {
                    caller.Notify("[Command] Invalid mob.");
                }
            }
        }
    }
}
