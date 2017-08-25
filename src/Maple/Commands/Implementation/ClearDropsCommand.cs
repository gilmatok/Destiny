using Destiny.Maple.Characters;
using Destiny.Maple.Maps;
using System.Collections.Generic;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class ClearDropsCommand : Command
    {
        public override string Name
        {
            get
            {
                return "cleardrops";
            }
        }

        public override string Parameters
        {
            get
            {
                return "[ -pickup ]";
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
            if (args.Length > 1)
            {
                this.ShowSyntax(caller);
            }
            else
            {
                bool pickUp = false;

                if (args.Length == 1)
                {
                    if (args[0].ToLower() == "-pickup")
                    {
                        pickUp = true;
                    }
                    else
                    {
                        this.ShowSyntax(caller);

                        return;
                    }
                }

                lock (caller.Map.Drops)
                {
                    List<Drop> toPick = new List<Drop>();

                    foreach (Drop loopDrop in caller.Map.Drops)
                    {
                        toPick.Add(loopDrop);
                    }

                    foreach (Drop loopDrop in toPick)
                    {
                        if (pickUp)
                        {
                            loopDrop.Picker = caller;
                        }

                        caller.Map.Drops.Remove(loopDrop);
                    }
                }
            }
        }
    }
}
