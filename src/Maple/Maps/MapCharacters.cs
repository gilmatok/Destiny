using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Maple.Characters;
using Destiny.Maple.Life;
using Destiny.Maple.Social;
using System.Collections.Generic;

namespace Destiny.Maple.Maps
{
    public sealed class MapCharacters : MapObjects<Character>
    {
        public MapCharacters(Map map) : base(map) { }

        public Character this[string name]
        {
            get
            {
                foreach (Character character in this)
                {
                    if (character.Name.ToLower() == name.ToLower())
                    {
                        return character;
                    }
                }

                throw new KeyNotFoundException();
            }
        }

        protected override void InsertItem(int index, Character item)
        {
            lock (this)
            {
                foreach (Character character in this)
                {
                    using (OutPacket oPacket = character.GetSpawnPacket())
                    {
                        item.Client.Send(oPacket);
                    }
                }
            }

            item.Position = this.Map.Portals.Count > 0 ? this.Map.Portals[item.SpawnPoint].Position : new Point(0, 0);

            base.InsertItem(index, item);

            lock (this.Map.Drops)
            {
                foreach (Drop drop in this.Map.Drops)
                {
                    if (drop.Owner == null)
                    {
                        using (OutPacket oPacket = drop.GetSpawnPacket(item))
                        {
                            item.Client.Send(oPacket);
                        }
                    }
                    else
                    {
                        using (OutPacket oPacket = drop.GetSpawnPacket())
                        {
                            item.Client.Send(oPacket);
                        }
                    }
                }
            }

            lock (this.Map.Mobs)
            {
                foreach (Mob mob in this.Map.Mobs)
                {
                    using (OutPacket oPacket = mob.GetSpawnPacket())
                    {
                        item.Client.Send(oPacket);
                    }
                }
            }

            lock (this.Map.Npcs)
            {
                foreach (Npc npc in this.Map.Npcs)
                {
                    using (OutPacket oPacket = npc.GetSpawnPacket())
                    {
                        item.Client.Send(oPacket);
                    }
                }
            }

            lock (this.Map.Reactors)
            {
                foreach (Reactor reactor in this.Map.Reactors)
                {
                    using (OutPacket oPacket = reactor.GetSpawnPacket())
                    {
                        item.Client.Send(oPacket);
                    }
                }
            }

            lock (this.Map.Mobs)
            {
                foreach (Mob mob in this.Map.Mobs)
                {
                    mob.AssignController();
                }
            }

            lock (this.Map.Npcs)
            {
                foreach (Npc npc in this.Map.Npcs)
                {
                    npc.AssignController();
                }
            }

            using (OutPacket oPacket = item.GetCreatePacket())
            {
                this.Map.Broadcast(oPacket, item);
            }

            // TODO: Move else-where (maybe ChangeMap).
            if (item.Party != null)
            {
                // TODO: Update party's map.

                foreach (PartyMember member in item.Party)
                {
                    if (member.Character != item && member.Character != null && member.Character.Map.MapleID == this.Map.MapleID)
                    {
                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.RecieveHP))
                        {
                            oPacket
                                .WriteInt(member.Character.ID)
                                .WriteInt(member.Character.Health)
                                .WriteInt(member.Character.MaxHealth);

                            item.Client.Send(oPacket);
                        }

                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.RecieveHP))
                        {
                            oPacket
                                .WriteInt(item.ID)
                                .WriteInt(item.Health)
                                .WriteInt(item.MaxHealth);

                            member.Character.Client.Send(oPacket);
                        }
                    }
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (this)
            {
                Character item = base.Items[index];

                item.ControlledMobs.Clear();
                item.ControlledNpcs.Clear();

                using (OutPacket oPacket = item.GetDestroyPacket())
                {
                    this.Map.Broadcast(oPacket, item);
                }
            }

            base.RemoveItem(index);

            lock (this.Map.Mobs)
            {
                foreach (Mob mob in this.Map.Mobs)
                {
                    mob.AssignController();
                }
            }

            lock (this.Map.Npcs)
            {
                foreach (Npc npc in this.Map.Npcs)
                {
                    npc.AssignController();
                }
            }
        }

        protected override int GetKeyForItem(Character item)
        {
            return item.ID;
        }
    }
}
