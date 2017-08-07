using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Maple.Maps;

namespace Destiny.Maple.Life.Reactors
{
    public sealed class Reactor : MapObject, ISpawnable
    {
        public int MapleID { get; private set; }
        public string Label { get; private set; }
        public byte State { get; set; }
        public SpawnPoint SpawnPoint { get; private set; }
        public ReactorState[] States { get; set; }

        public Reactor CachedReference
        {
            get
            {
                return DataProvider.Reactors[this.MapleID];
            }
        }

        public Reactor(Datum datum)
            : base()
        {
            this.MapleID = (int)datum["reactorid"];
            this.Label = string.Empty; // TODO: Is this even relevant?
            this.State = 0;
            this.States = new ReactorState[(sbyte)datum["max_states"]];
        }

        public Reactor(int mapleID)
        {
            this.MapleID = mapleID;
            this.Label = this.CachedReference.Label;
            this.State = this.CachedReference.State;
            this.States = this.CachedReference.States;
        }

        public Reactor(SpawnPoint spawnPoint)
            : this(spawnPoint.MapleID)
        {
            this.SpawnPoint = spawnPoint;
            this.Position = spawnPoint.Position;
        }

        public void Hit(Character character, short actionDelay, int skillID)
        {
            ReactorState state = this.States[this.State];

            switch (state.Type)
            {
                case ReactorEventType.PlainAdvanceState:
                    {
                        this.State = state.NextState;

                        if (this.State == this.States.Length - 1) // TODO: Is this the correct way of doing this?
                        {
                            this.Map.Reactors.Remove(this);
                        }
                        else
                        {
                            using (OutPacket oPacket = new OutPacket(ServerOperationCode.ReactorChangeState))
                            {
                                oPacket
                                    .WriteInt(this.ObjectID)
                                    .WriteByte(this.State)
                                    .WritePoint(this.Position)
                                    .WriteShort(actionDelay)
                                    .WriteByte() // NOTE: Event index.
                                    .WriteByte(4); // NOTE: Delay.

                                this.Map.Broadcast(oPacket);
                            }
                        }
                    }
                    break;
            }
        }

        public OutPacket GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public OutPacket GetSpawnPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.ReactorEnterField);

            oPacket
                .WriteInt(this.ObjectID)
                .WriteInt(this.MapleID)
                .WriteByte(this.State)
                .WritePoint(this.Position)
                .WriteShort() // NOTE: Flags (not sure).
                .WriteBool() // NOTE: Unknown
                .WriteString(this.Label);

            return oPacket;
        }

        public OutPacket GetDestroyPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.ReactorLeaveField);

            oPacket
                .WriteInt(this.ObjectID)
                .WriteByte(this.State)
                .WritePoint(this.Position);

            return oPacket;
        }
    }
}
