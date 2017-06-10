using Destiny.Game.Characters;
using Destiny.Server;
using System;

namespace Destiny.Game.Commands
{
    public sealed class ItemCommand : Command
    {
        public override string Name
        {
            get
            {
                return "item";
            }
        }

        public override string Parameters
        {
            get
            {
                return "{ id } [ quantity ]";
            }
        }

        public override GmLevel RequiredLevel
        {
            get
            {
                return GmLevel.Gm;
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
                short quantity = 0;
                bool isQuantitySpecified;

                if (args.Length > 1)
                {
                    isQuantitySpecified = short.TryParse(args[args.Length - 1], out quantity);
                }
                else
                {
                    isQuantitySpecified = false;
                }

                if (quantity < 1)
                {
                    quantity = 1;
                }

                int itemID = -1;

                try
                {
                    itemID = int.Parse(args[0]);
                }
                catch (FormatException)
                {
                    itemID = -1; // TODO: Obtain from strings.
                }

                if (MasterServer.Instance.Items.IsValidItem(itemID) ||
                    MasterServer.Instance.Equips.IsValidEquip(itemID))
                {
                    caller.Items.Add(itemID, quantity);
                }
                else
                {
                    caller.Notify("[Command] Invalid item.");
                }
            }
        }
    }
}
