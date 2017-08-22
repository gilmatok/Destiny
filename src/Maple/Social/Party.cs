using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Maple.Characters;
using Destiny.Server;
using System.Collections.Generic;

namespace Destiny.Maple.Social
{
    public sealed class Party
    {
        public WorldServer World { get; private set; }

        public int ID { get; private set; }
        public int LeaderID { get; private set; }
        public Dictionary<int, PartyMember> Members { get; private set; }
        public Dictionary<int, Character> Characters { get; private set; }

        public int Count
        {
            get
            {
                return this.Members.Count;
            }
        }

        public bool IsFull
        {
            get
            {
                return this.Members.Count == 6;
            }
        }

        public Party(WorldServer world, int id, Character leader)
        {
            this.World = world;

            this.ID = id;
            this.LeaderID = leader.ID;
            this.Members = new Dictionary<int, PartyMember>();
            this.Characters = new Dictionary<int, Character>();

            this.AddMember(leader, true);

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

        public void UpdateLeader(int leaderID, bool fromDisconnection = false)
        {
            this.LeaderID = leaderID;

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.PartyResult))
            {
                oPacket
                    .WriteByte((byte)PartyResult.ChangeLeader)
                    .WriteInt(this.LeaderID)
                    .WriteBool(fromDisconnection);

                this.Broadcast(oPacket);
            }
        }

        public void AddMember(Character member, bool isFirst = false)
        {
            member.Party = this;

            this.Members[member.ID] = new PartyMember(this, member);
            this.Characters[member.ID] = member;

            if (!isFirst)
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.PartyResult))
                {
                    oPacket
                        .WriteByte((byte)PartyResult.Join)
                        .WriteInt(this.ID)
                        .WriteMapleString(member.Name)
                        .WriteBytes(this.ToByteArray());

                    this.Broadcast(oPacket);
                }

                foreach (Character character in this.Characters.Values)
                {
                    if (character != member && character.Map.MapleID == member.Map.MapleID)
                    {
                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.UpdatePartyMemberHP))
                        {
                            oPacket
                                .WriteInt(member.ID)
                                .WriteInt(member.Health)
                                .WriteInt(member.MaxHealth);

                            character.Client.Send(oPacket);
                        }

                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.UpdatePartyMemberHP))
                        {
                            oPacket
                                .WriteInt(character.ID)
                                .WriteInt(character.Health)
                                .WriteInt(character.MaxHealth);

                            member.Client.Send(oPacket);
                        }
                    }
                }
            }
        }

        public void SilentRemoveMember(Character character)
        {
            this.Characters.Remove(character.ID);

            this.Members[character.ID].Map = 999999999;
            this.Members[character.ID].Channel = -2;

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.PartyResult))
            {
                oPacket
                    .WriteByte((byte)PartyResult.Update)
                    .WriteInt(this.ID)
                    .WriteBytes(this.ToByteArray());

                this.Broadcast(oPacket);
            }
        }

        public void HardRemoveMember(Character member, bool expel = false)
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.PartyResult))
            {
                oPacket
                    .WriteByte((byte)PartyResult.RemoveOrLeave)
                    .WriteInt(this.ID)
                    .WriteInt(member.ID)
                    .WriteBool(true)
                    .WriteBool(expel)
                    .WriteMapleString(member.Name)
                    .WriteBytes(this.ToByteArray());

                this.Broadcast(oPacket);
            }

            this.Members.Remove(member.ID);
            this.Characters.Remove(member.ID);

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.PartyResult))
            {
                oPacket
                    .WriteByte((byte)PartyResult.Update)
                    .WriteInt(this.ID)
                    .WriteBytes(this.ToByteArray());

                this.Broadcast(oPacket);
            }

            member.Party = null;
        }

        public void Broadcast(OutPacket oPacket, Character ignored = null)
        {
            foreach (Character member in this.Characters.Values)
            {
                if (member != ignored)
                {
                    member.Client.Send(oPacket);
                }
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

            foreach (Character member in this.Characters.Values)
            {
                member.Party = null;
            }

            this.World.RemoveParty(this);
        }

        public byte[] ToByteArray()
        {
            using (OutPacket oPacket = new OutPacket())
            {
                // NOTE: IDs.

                foreach (PartyMember member in this.Members.Values)
                {
                    oPacket.WriteInt(member.ID);
                }

                int remaining = 6 - this.Count;

                for (int i = 0; i < remaining; i++)
                {
                    oPacket.WriteInt();
                }

                // NOTE: Names.

                foreach (PartyMember member in this.Members.Values)
                {
                    oPacket.WritePaddedString(member.Name, 13);
                }

                for (int i = 0; i < remaining; i++)
                {
                    oPacket.Skip(13);
                }

                // NOTE: Jobs.

                foreach (PartyMember member in this.Members.Values)
                {
                    oPacket.WriteInt((int)member.Job);
                }

                for (int i = 0; i < remaining; i++)
                {
                    oPacket.WriteInt();
                }

                // NOTE: Levels.

                foreach (PartyMember member in this.Members.Values)
                {
                    oPacket.WriteInt(member.Level);
                }

                for (int i = 0; i < remaining; i++)
                {
                    oPacket.WriteInt();
                }

                // NOTE: Channels.

                foreach (PartyMember member in this.Members.Values)
                {
                    oPacket.WriteInt(member.Channel);
                }

                for (int i = 0; i < remaining; i++)
                {
                    oPacket.WriteInt(-2);
                }

                oPacket.WriteInt(this.LeaderID);

                // NOTE: Maps.

                foreach (PartyMember member in this.Members.Values)
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
    }
}