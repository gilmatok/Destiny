using Destiny.Maple.Characters;
using Destiny.Network;
using System;
using System.Collections.Generic;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class AdminShopCommand : Command
    {
        // NOTE: The Npc that is the shop owner.
        public const int Npc = 2084001;

        // TODO: Make a separate class called AdminShopItem to hold these values.
        // We can either make the items constant or load them from SQL.
        // As you can edit them in-game, I think SQL would be better.
        // In order: ID, MapleID, Price, Stock.
        public static List<Tuple<int, int, int, short>> Items = new List<Tuple<int, int, int, short>>()
        {
            new Tuple<int, int, int, short>(0, 2000000, 1000, 200),
            new Tuple<int, int, int, short>(1, 2000001, 1000, 200),
            new Tuple<int, int, int, short>(2, 2000002, 1000, 200)
        };

        public override string Name
        {
            get
            {
                return "adminshop";
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
                using (Packet oPacket = new Packet(ServerOperationCode.AdminShop))
                {
                    oPacket
                        .WriteInt(Npc)
                        .WriteShort((short)AdminShopCommand.Items.Count);

                    foreach (var item in AdminShopCommand.Items)
                    {
                        oPacket
                            .WriteInt(item.Item1)
                            .WriteInt(item.Item2)
                            .WriteInt(item.Item3)
                            .WriteByte() // NOTE: Unknown.
                            .WriteShort(item.Item4);
                    }

                    // NOTE: If enabled, when you exit the shop the NPC will ask you if you were looking for something that was missing.
                    // If you press yes, a search box with all the items in game will pop up and you can select an item to "register".
                    // Once you register an item, a packet will be sent to the server with it's ID so it can be added to the shop.
                    oPacket.WriteBool(true);

                    caller.Client.Send(oPacket);
                }
            }
        }
    }
}
