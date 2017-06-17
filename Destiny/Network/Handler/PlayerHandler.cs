using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Maple;
using Destiny.Maple.Characters;
using Destiny.Maple.Commands;
using Destiny.Maple.Maps;
using Destiny.Server;
using System.Collections.Generic;

namespace Destiny.Handler
{
    public static class PlayerHandler
    {
        public static void HandleMapChange(MapleClient client, InPacket iPacket)
        {
            byte portals = iPacket.ReadByte();

            if (portals != client.Character.Portals)
            {
                return;
            }

            int destinationID = iPacket.ReadInt();

            switch (destinationID)
            {
                case -1:
                    {
                        string label = iPacket.ReadMapleString();
                        Portal portal;

                        try
                        {
                            portal = client.Character.Map.Portals[label];
                        }
                        catch (KeyNotFoundException)
                        {
                            return;
                        }

                        client.Character.ChangeMap(portal.DestinationMap, portal.Link.ID);
                    }
                    break;
            }
        }

        public static void HandlePlayerChat(MapleClient client, InPacket iPacket)
        {
            string text = iPacket.ReadMapleString();
            bool shout = iPacket.ReadBool(); // NOTE: Used for skill macros.

            if (text.StartsWith(Constants.CommandIndiciator.ToString()))
            {
                CommandFactory.Execute(client.Character, text);
            }
            else
            {
                using (OutPacket oPacket = new OutPacket(SendOps.UserChat))
                {
                    oPacket
                        .WriteInt(client.Character.ID)
                        .WriteBool(client.Character.IsGm)
                        .WriteMapleString(text)
                        .WriteBool(shout);

                    client.Character.Map.Broadcast(oPacket);
                }
            }
        }

        public static void HandlePlayerMovement(MapleClient client, InPacket iPacket)
        {
            //Movements movements = Movements.Decode(iPacket);

            //// TODO: Validate movements.

            //Movement lastMovement = movements[movements.Count - 1];

            //client.Character.Position = lastMovement.Position;
            //client.Character.Foothold = lastMovement.Foothold;
            //client.Character.Stance = lastMovement.Stance;

            //using (OutPacket oPacket = new OutPacket(SendOps.UserMove))
            //{
            //    oPacket.WriteInt(client.Character.ID);
            //    movements.Encode(oPacket);

            //    client.Character.Map.Broadcast(oPacket, client.Character);
            //}
        }

        public static void HandlePlayerInformation(MapleClient client, InPacket iPacket)
        {
            //iPacket.Skip(4);
            //int characterID = iPacket.ReadInt();

            //Character target;

            //try
            //{
            //    target = client.Character.Map.Characters[characterID];
            //}
            //catch (KeyNotFoundException)
            //{
            //    return;
            //}

            //if (target.IsGm)
            //{
            //    return;
            //}

            //using (OutPacket oPacket = new OutPacket(SendOps.CharacterInformation))
            //{
            //    oPacket
            //        .WriteInt(target.ID)
            //        .WriteByte(target.Stats.Level)
            //        .WriteShort((short)target.Stats.Job)
            //        .WriteShort(target.Stats.Fame)
            //        .WriteBool() // NOTE: Marriage.
            //        .WriteMapleString("-") // NOTE: Guild name.
            //        .WriteMapleString("-") // NOTE: Alliance name.
            //        .WriteByte() // NOTE: Unknown.
            //        .WriteByte() // NOTE: Pets.
            //        .WriteByte() // NOTE: Mount.
            //        .WriteByte() // NOTE: Wishlist.
            //        .WriteInt() // NOTE: Monster Book level.
            //        .WriteInt() // NOTE: Monster Book normal cards. 
            //        .WriteInt() // NOTE: Monster Book special cards.
            //        .WriteInt() // NOTE: Monster Book total cards.
            //        .WriteInt() // NOTE: Monster Book cover.
            //        .WriteInt() // NOTE: Medal ID.
            //        .WriteShort(); // NOTE: Medal quests.

            //    client.Send(oPacket);
            //}
        }
    }
}
