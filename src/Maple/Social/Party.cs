using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Maple.Characters;
using Destiny.Server;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Social
{
    public sealed class Party : KeyedCollection<int, PartyMember>
    {
        public WorldServer World { get; private set; }

        public int ID { get; private set; }
        public int LeaderID { get; private set; }

        public bool IsFull
        {
            get
            {
                return this.Count == 6;
            }
        }

        public Party(WorldServer world, int id, Character leader)
            : base()
        {
            this.World = world;

            this.ID = id;
            this.LeaderID = leader.ID;

            this.Add(new PartyMember(leader));

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.PartyResult))
            {
                oPacket
                    .WriteByte((byte)PartyResult.Create)
                    .WriteInt(this.ID)
                    .WriteInt(999999999)
                    .WriteInt(999999999)
                    .WriteShort()
                    .WriteShort();

                leader.Client.Send(oPacket);
            }
        }

        public void Update()
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.PartyResult))
            {
                oPacket
                    .WriteByte((byte)PartyResult.Update)
                    .WriteInt(this.ID)
                    .WriteBytes(this.ToByteArray());

                this.Broadcast(oPacket);
            }
        }

        public void Disband()
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.PartyResult))
            {
                oPacket
                    .WriteByte((byte)PartyResult.RemoveOrLeave)
                    .WriteInt(this.ID)
                    .WriteInt(this.LeaderID)
                    .WriteBool(false);

                this.Broadcast(oPacket);
            }

            foreach (PartyMember member in this)
            {
                if (member.Character != null)
                {
                    member.Character.Party = null;
                }
            }

            this.World.RemoveParty(this);
        }

        public void Broadcast(OutPacket oPacket, PartyMember ignored = null)
        {
            foreach (PartyMember member in this)
            {
                if (member.Character != null)
                {
                    if (member != ignored)
                    {
                        member.Character.Client.Send(oPacket);
                    }
                }
            }
        }

        public byte[] ToByteArray()
        {
            using (OutPacket oPacket = new OutPacket())
            {
                int remaining = 6 - this.Count;

                // NOTE: IDs.

                foreach (PartyMember member in this)
                {
                    oPacket.WriteInt(member.ID);
                }

                for (int i = 0; i < remaining; i++)
                {
                    oPacket.WriteInt();
                }

                // NOTE: Names.

                foreach (PartyMember member in this)
                {
                    oPacket.WritePaddedString(member.Name, 13);
                }

                for (int i = 0; i < remaining; i++)
                {
                    oPacket.Skip(13);
                }

                // NOTE: Jobs.

                foreach (PartyMember member in this)
                {
                    oPacket.WriteInt((int)member.Job);
                }

                for (int i = 0; i < remaining; i++)
                {
                    oPacket.WriteInt();
                }

                // NOTE: Levels.

                foreach (PartyMember member in this)
                {
                    oPacket.WriteInt(member.Level);
                }

                for (int i = 0; i < remaining; i++)
                {
                    oPacket.WriteInt();
                }

                // NOTE: Channels.

                foreach (PartyMember member in this)
                {
                    oPacket.WriteInt(member.Channel);
                }

                for (int i = 0; i < remaining; i++)
                {
                    oPacket.WriteInt(-2);
                }

                oPacket.WriteInt(this.LeaderID);

                // NOTE: Maps.

                foreach (PartyMember member in this)
                {
                    oPacket.WriteInt(member.Map);
                }

                for (int i = 0; i < remaining; i++)
                {
                    oPacket.WriteInt(999999999);
                }

                // NOTE: Doors.
                {
                    for (int i = 0; i < 6; i++)
                    {
                        oPacket
                            .WriteInt(999999999) // NOTE: Town ID.
                            .WriteInt(999999999) // NOTE: Target ID.
                            .WriteInt(-1) // NOTE: Position X.
                            .WriteInt(-1); // NOTE: Position Y.
                    }
                }

                return oPacket.ToArray();
            }
        }

        protected override int GetKeyForItem(PartyMember item)
        {
            return item.ID;
        }

        protected override void InsertItem(int index, PartyMember item)
        {
            item.Party = this;
            item.Character.Party = this;

            base.InsertItem(index, item);

            if (this.Count > 1)
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.PartyResult))
                {
                    oPacket
                        .WriteByte((byte)PartyResult.Join)
                        .WriteInt(this.ID)
                        .WriteMapleString(item.Name)
                        .WriteBytes(this.ToByteArray());

                    this.Broadcast(oPacket);
                }

                foreach (PartyMember member in this)
                {
                    if (member != item && member.Character != null && member.Character.Map.MapleID == item.Character.Map.MapleID)
                    {
                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.UpdatePartyMemberHP))
                        {
                            oPacket
                                .WriteInt(member.Character.ID)
                                .WriteInt(member.Character.Health)
                                .WriteInt(member.Character.MaxHealth);

                            item.Character.Client.Send(oPacket);
                        }

                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.UpdatePartyMemberHP))
                        {
                            oPacket
                                .WriteInt(item.Character.ID)
                                .WriteInt(item.Character.Health)
                                .WriteInt(item.Character.MaxHealth);

                            member.Character.Client.Send(oPacket);
                        }
                    }
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            PartyMember item = base.Items[index];

            item.Party = null;

            if (item.Character != null)
            {
                item.Character.Party = null;
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.PartyResult))
            {
                oPacket
                    .WriteByte((byte)PartyResult.RemoveOrLeave)
                    .WriteInt(this.ID)
                    .WriteInt(item.ID)
                    .WriteBool(true)
                    .WriteBool(item.Expelled)
                    .WriteMapleString(item.Name)
                    .WriteBytes(this.ToByteArray());

                this.Broadcast(oPacket);
            }

            base.RemoveItem(index);

            this.Update();
        }
    }
}