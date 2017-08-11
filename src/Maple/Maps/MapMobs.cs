using Destiny.Core.IO;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Maple.Life;
using Destiny.Network;
using Destiny.Threading;
using System;
using System.Collections.Generic;

namespace Destiny.Maple.Maps
{
    public sealed class MapMobs : MapObjects<Mob>
    {
        public MapMobs(Map map) : base(map) { }

        protected override void InsertItem(int index, Mob item)
        {
            base.InsertItem(index, item);

            if (DataProvider.IsInitialized)
            {
                using (OutPacket oPacket = item.GetCreatePacket())
                {
                    this.Map.Broadcast(oPacket);
                }

                item.AssignController();
            }
        }

        protected override void RemoveItem(int index) // NOTE: Equivalent of mob death.
        {
            Mob item = base.Items[index];

            int mostDamage = 0;
            Character owner = null;

            foreach (KeyValuePair<Character, uint> attacker in item.Attackers)
            {
                if (attacker.Key.Map == this.Map)
                {
                    if (attacker.Value > mostDamage)
                    {
                        owner = attacker.Key;
                    }

                    attacker.Key.Experience += (int)Math.Min(item.Experience, (attacker.Value * item.Experience) / item.MaxHealth) * MasterServer.World.ExperienceRate;
                }
            }

            item.Attackers.Clear();

            if (item.CanDrop)
            {
                List<Drop> drops = new List<Drop>();

                foreach (Loot loopLoot in item.Loots)
                {
                    if ((Constants.Random.Next(1000000) / MasterServer.World.DropRate) <= loopLoot.Chance)
                    {
                        if (loopLoot.IsMeso)
                        {
                            drops.Add(new Meso((short)(Constants.Random.Next(loopLoot.MinimumQuantity, loopLoot.MaximumQuantity) * MasterServer.World.MesoRate))
                            {
                                Dropper = item,
                                Owner = owner
                            });
                        }
                        else
                        {
                            drops.Add(new Item(loopLoot.MapleID, (short)Constants.Random.Next(loopLoot.MinimumQuantity, loopLoot.MaximumQuantity))
                            {
                                Dropper = item,
                                Owner = owner
                            });
                        }
                    }
                }

                double i = drops.Count / 2.0 * -1;

                foreach (Drop loopDrop in drops)
                {
                    i++;

                    this.Map.Drops.Add(loopDrop);
                }
            }

            if (owner != null)
            {
                // TODO: Update quest kills if quest is started.
                // TODO: Check if owner can complete quest.                
            }

            if (DataProvider.IsInitialized)
            {
                item.Controller.ControlledMobs.Remove(item);

                using (OutPacket oPacket = item.GetDestroyPacket())
                {
                    this.Map.Broadcast(oPacket);
                }
            }

            base.RemoveItem(index);

            if (item.SpawnPoint != null)
            {
                Delay.Execute(3 * 1000, () => item.SpawnPoint.Spawn());
            }

            foreach (int summonID in item.DeathSummons)
            {
                this.Map.Mobs.Add(new Mob(summonID)
                {
                    Position = item.Position // TODO: Set owner as well.
                });
            }
        }
    }
}
