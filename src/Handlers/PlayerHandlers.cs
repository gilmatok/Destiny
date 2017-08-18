using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Maple;
using Destiny.Maple.Commands;
using Destiny.Maple.Life;
using Destiny.Maple.Maps;
using System.Collections.Generic;

namespace Destiny.Handlers
{
    public static class PlayerHandlers
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

                        client.Character.ChangeMap(portal.DestinationMapID, portal.Link.ID);
                    }
                    break;
            }
        }

        public static void HandleMovement(MapleClient client, InPacket iPacket)
        {
            byte portals = iPacket.ReadByte();

            if (portals != client.Character.Portals)
            {
                return;
            }

            iPacket.ReadInt(); // NOE: Unknown.

            Movements movements = Movements.Decode(iPacket);

            client.Character.Position = movements.Position;
            client.Character.Foothold = movements.Foothold;
            client.Character.Stance = movements.Stance;

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.UserMove))
            {
                oPacket.WriteInt(client.Character.ID);

                movements.Encode(oPacket);

                client.Character.Map.Broadcast(oPacket, client.Character);
            }

            if (client.Character.Foothold == 0)
            {
                // NOTE: Player is floating in the air.
                // GMs might be legitmately in this state due to GM fly.
                // We shouldn't mess with them because they have the tools toget out of falling off the map anyway.

                // TODO: Attempt to find foothold.
                // If none found, check the player fall counter.
                // If it's over 3, reset the player's map.
            }
        }

        public static void HandleMeleeAttack(MapleClient client, InPacket iPacket)
        {
            Attack attack = new Attack(iPacket, AttackType.Melee);

            if (attack.Portals != client.Character.Portals)
            {
                return;
            }

            Skill skill = null;

            if (attack.SkillID > 0)
            {
                skill = client.Character.Skills[attack.SkillID];

                skill.Cast();
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CloseRangeAttack))
            {
                oPacket
                    .WriteInt(client.Character.ID)
                    .WriteByte((byte)((attack.Targets * 0x10) + attack.Hits))
                    .WriteByte() // NOTE: Unknown.
                    .WriteByte((byte)(attack.SkillID != 0 ? skill.CurrentLevel : 0)); // NOTE: Skill level.

                if (attack.SkillID != 0)
                {
                    oPacket.WriteInt(attack.SkillID);
                }

                oPacket
                    .WriteByte() // NOTE: Unknown.
                    .WriteByte(attack.Display)
                    .WriteByte(attack.Animation)
                    .WriteByte(attack.WeaponSpeed)
                    .WriteByte() // NOTE: Skill mastery.
                    .WriteInt(); // NOTE: Unknown.

                foreach (var target in attack.Damages)
                {
                    oPacket
                        .WriteInt(target.Key)
                        .WriteByte(6);

                    foreach (uint hit in target.Value)
                    {
                        oPacket.WriteUInt(hit);
                    }
                }

                client.Character.Map.Broadcast(oPacket, client.Character);
            }

            foreach (KeyValuePair<int, List<uint>> target in attack.Damages)
            {
                Mob mob;

                try
                {
                    mob = client.Character.Map.Mobs[target.Key];
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }

                mob.IsProvoked = true;
                mob.SwitchController(client.Character);

                foreach (uint hit in target.Value)
                {
                    if (mob.Damage(client.Character, hit))
                    {
                        mob.Die();
                    }
                }
            }
        }

        private const sbyte BumpDamage = -1;
        private const sbyte MapDamage = -2;

        public static void HandleHit(MapleClient client, InPacket iPacket)
        {
            iPacket.Skip(4); // NOTE: Ticks.
            sbyte type = (sbyte)iPacket.ReadByte();
            iPacket.ReadByte(); // NOTE: Elemental type.
            int damage = iPacket.ReadInt();
            bool damageApplied = false;
            bool deadlyAttack = false;
            byte hit = 0;
            byte stance = 0;
            int disease = 0;
            byte level = 0;
            short mpBurn = 0;
            int mobObjectID = 0;
            int mobID = 0;
            int noDamageSkillID = 0;

            if (type != MapDamage)
            {
                mobID = iPacket.ReadInt();
                mobObjectID = iPacket.ReadInt();

                Mob mob;

                try
                {
                    mob = client.Character.Map.Mobs[mobObjectID];
                }
                catch (KeyNotFoundException)
                {
                    return;
                }

                if (mobID != mob.MapleID)
                {
                    return;
                }

                if (type != BumpDamage)
                {
                    // TODO: Get mob attack and apply to disease/level/mpBurn/deadlyAttack.
                }
            }

            hit = iPacket.ReadByte();
            byte reduction = iPacket.ReadByte();
            iPacket.ReadByte(); // NOTE: Unknown.

            if (reduction != 0)
            {
                // TODO: Return damage (Power Guard).
            }

            if (type == MapDamage)
            {
                level = iPacket.ReadByte();
                disease = iPacket.ReadInt();
            }
            else
            {
                stance = iPacket.ReadByte();

                if (stance > 0)
                {
                    // TODO: Power Stance.
                }
            }

            if (damage == -1)
            {
                // TODO: Validate no damage skills.
            }

            if (disease > 0 && damage != 0)
            {
                // NOTE: Fake/Guardian don't prevent disease.
                // TODO: Add disease buff.
            }

            if (damage > 0)
            {
                // TODO: Check for Meso Guard.
                // TODO: Check for Magic Guard.
                // TODO: Check for Achilles.

                if (!damageApplied)
                {
                    if (deadlyAttack)
                    {
                        // TODO: Deadly attack function.
                    }
                    else
                    {
                        client.Character.Health -= (short)damage;
                    }

                    if (mpBurn > 0)
                    {
                        client.Character.Mana -= (short)mpBurn;
                    }
                }

                // TODO: Apply damage to buffs.
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.UserHit))
            {
                oPacket
                    .WriteInt(client.Character.ID)
                    .WriteSByte(type);

                switch (type)
                {
                    case MapDamage:
                        {
                            oPacket
                                .WriteInt(damage)
                                .WriteInt(damage);
                        }
                        break;

                    default:
                        {
                            oPacket
                                .WriteInt(damage) // TODO: ... or PGMR damage.
                                .WriteInt(mobID)
                                .WriteByte(hit)
                                .WriteByte(reduction);

                            if (reduction > 0)
                            {
                                // TODO: PGMR stuff.
                            }

                            oPacket
                                .WriteByte(stance)
                                .WriteInt(damage);

                            if (noDamageSkillID > 0)
                            {
                                oPacket.WriteInt(noDamageSkillID);
                            }
                        }
                        break;
                }

                client.Character.Map.Broadcast(oPacket, client.Character);
            }
        }

        public static void HandleChat(MapleClient client, InPacket iPacket)
        {
            string text = iPacket.ReadMapleString();
            bool shout = iPacket.ReadBool(); // NOTE: Used for skill macros.

            if (text.StartsWith(Constants.CommandIndiciator.ToString()))
            {
                CommandFactory.Execute(client.Character, text);
            }
            else
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.UserChat))
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

        public static void HandleFacialExpression(MapleClient client, InPacket iPacket)
        {
            int expressionID = iPacket.ReadInt();

            if (expressionID > 7) // NOTE: Cash facial expression.
            {
                int mapleID = 5159992 + expressionID;

                if (!client.Character.Items.Contains(mapleID))
                {
                    return;
                }
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.UserEmotion))
            {
                oPacket
                    .WriteInt(client.Character.ID)
                    .WriteInt(expressionID);

                client.Character.Map.Broadcast(oPacket, client.Character);
            }
        }
    }
}
