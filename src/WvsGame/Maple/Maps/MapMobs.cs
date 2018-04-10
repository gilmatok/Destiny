using Destiny.Network;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Maple.Life;
using System;
using System.Collections.Generic;
using Destiny.IO;
using Destiny.Threading;

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
                using (Packet oPacket = item.GetCreatePacket())
                {
                    this.Map.Broadcast(oPacket);
                }

                try
                {
                    item.AssignController();
                }
                catch (Exception e)
                {
                    Log.SkipLine();
                    Log.Inform("ERROR: MapMobs-InsertItem() failed to AssignController to mobObject item: {0}! \n Exception occured: {1}", item.ObjectID, e);
                    Log.SkipLine();
                }
            }
        }

        //TODO: remake as MobDied(int index, bool isBoss)
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
                    //TODO: more conditioning, exps buffs, exps debuffs
                    attacker.Key.Experience += (int)Math.Min(item.Experience, (attacker.Value * item.Experience) / item.MaxHealth) * WvsGame.ExperienceRate;
                    int expGained = (int)item.Experience * WvsGame.ExperienceRate;
                    Packet ExpGainOnMobDeath = Experience.GetShowExpGainPacket(true, expGained, false, 0, 0);
                    owner.Client.Send(ExpGainOnMobDeath);
                }
            }

            item.Attackers.Clear();

            if (item.CanDrop)
            {
                List<Drop> drops = new List<Drop>();

                foreach (Loot loopLoot in item.Loots)
                {
                    if ((Application.Random.Next(1000000) / WvsGame.DropRate) <= loopLoot.Chance)
                    {
                        if (loopLoot.IsMeso)
                        {
                            drops.Add(new Meso((short)(Application.Random.Next(loopLoot.MinimumQuantity, loopLoot.MaximumQuantity) * WvsGame.MesoRate))
                            {
                                Dropper = item,
                                Owner = owner
                            });
                        }
                        else
                        {
                            drops.Add(new Item(loopLoot.MapleID, (short)Application.Random.Next(loopLoot.MinimumQuantity, loopLoot.MaximumQuantity))
                            {
                                Dropper = item,
                                Owner = owner
                            });
                        }
                    }
                }

                foreach (Drop loopDrop in drops)
                {
                    // TODO: Space out drops.

                    this.Map.Drops.Add(loopDrop);
                }
            }

            if (owner != null)
            {
                foreach (KeyValuePair<ushort, Dictionary<int, short>> loopStarted in owner.Quests.Started)
                {
                    if (loopStarted.Value.ContainsKey(item.MapleID))
                    {
                        if (loopStarted.Value[item.MapleID] < DataProvider.Quests[loopStarted.Key].PostRequiredKills[item.MapleID])
                        {
                            loopStarted.Value[item.MapleID]++;

                            using (Packet oPacket = new Packet(ServerOperationCode.Message))
                            {
                                oPacket
                                    .WriteByte((byte)MessageType.QuestRecord)
                                    .WriteUShort(loopStarted.Key)
                                    .WriteByte(1);

                                string kills = string.Empty;

                                foreach (int kill in loopStarted.Value.Values)
                                {
                                    kills += kill.ToString().PadLeft(3, '0');
                                }

                                oPacket
                                    .WriteString(kills)
                                    .WriteInt()
                                    .WriteInt();

                                owner.Client.Send(oPacket);

                                if (owner.Quests.CanComplete(loopStarted.Key, true))
                                {
                                    owner.Quests.NotifyComplete(loopStarted.Key);
                                }
                            }
                        }
                    }
                }
            }

            if (DataProvider.IsInitialized)
            {
                if (item.Controller.ControlledMobs.Contains(item))
                {
                    try
                    {
                        item.Controller.ControlledMobs.Remove(item);
                    }

                    catch (Exception e)
                    {
                        Log.SkipLine();
                        Log.Inform("ERROR: failed to remove mobObject: {0} from controlledMobs! \n Exception occured: {1}", item.ObjectID, e);
                        Log.SkipLine();
                    }
                }

                using (Packet oPacket = item.GetDestroyPacket((Mob.DeathEffects)item.DeathEffect))
                {
                    this.Map.Broadcast(oPacket);
                }
            }

            base.RemoveItem(index);

            if (item.SpawnPoint != null)
            {
                Delay.Execute(() => item.SpawnPoint.Spawn(), 3 * 1000); // TODO: Actual respawn time.
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
