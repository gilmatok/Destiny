using Destiny.Maple.Characters;
using Destiny.Maple.Data;

namespace Destiny.Maple.Commands.Implementation
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
                short quantity = 0;

                if (args.Length > 1)
                {
                    short.TryParse(args[args.Length - 1], out quantity);
                }

                if (quantity < 1)
                {
                    quantity = 1;
                }

                int itemID = int.Parse(args[0]);

                if (DataProvider.Items.Contains(itemID))
                {
                    caller.Items.Add(new Item(itemID, quantity));
                }
                else
                {
                    caller.Notify("[Command] Invalid item.");
                }
            }
        }
    }
}
