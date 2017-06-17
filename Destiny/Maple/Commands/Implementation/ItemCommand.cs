using Destiny.Maple.Characters;
using Destiny.Server;

namespace Destiny.Maple.Commands
{
    public sealed class ItemCommand : Command
    {
        public override string Name => "item";
        public override string Parameters => "{ id } [ quantity ]";
        public override GmLevel RequiredLevel => GmLevel.Gm;

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

                int itemID = int.Parse(args[0]);



                //if (MasterServer.Instance.Items.IsValidItem(itemID) || MasterServer.Instance.Equips.IsValidEquip(itemID))
                //    caller.Items.Add(itemID, quantity);
                //else
                //    caller.Notify("[Command] Invalid item.");
            }
        }
    }
}
