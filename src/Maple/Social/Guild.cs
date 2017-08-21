using Destiny.Data;
using Destiny.Maple.Characters;
using System.Collections.ObjectModel;
using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Server;

namespace Destiny.Maple.Social
{
    public sealed class Guild : KeyedCollection<int, GuildMember>
    {
        public WorldServer World { get; private set; }

        public int ID { get; private set; }
        public int LeaderID { get; private set; }
        public string Name { get; private set; }
        public string[] Titles { get; private set; }
        public int Capacity { get; private set; }
        public string Notice { get; private set; }
        public int Points { get; private set; }
        public short Logo { get; private set; }
        public short Background { get; private set; }
        public byte LogoColor { get; private set; }
        public byte BackgroundColor { get; private set; }

        public bool IsFull
        {
            get
            {
                return this.Count == this.Capacity;
            }
        }

        private bool Assigned { get; set; }

        public Guild(WorldServer world, Datum datum)
        {
            this.World = world;

            this.ID = (int)datum["ID"];
            this.Assigned = true;

            this.LeaderID = (int)datum["LeaderID"];
            this.Name = (string)datum["Name"];

            // NOTE: Ugly, but works.
            this.Titles = new string[5];
            for (int i = 0; i < 5; i++)
            {
                this.Titles[i] = (string)datum["Title" + i];
            }

            this.Capacity = (int)datum["Capacity"];
            this.Notice = (string)datum["Notice"];
            this.Points = (int)datum["Points"];
            this.Logo = (short)datum["Logo"];
            this.Background = (short)datum["Background"];
            this.LogoColor = (byte)datum["LogoColor"];
            this.BackgroundColor = (byte)datum["BackgroundColor"];
        }

        public void Handle(GuildAction action, Character character, InPacket iPacket)
        {
            GuildMember member = this[character.ID];

            switch (action)
            {
                case GuildAction.Update:
                    {
                        // TODO: What does this do?
                    }
                    break;

                case GuildAction.Invite:
                    {
                        if (member.Rank > 2)
                        {
                            // NOTE: Only guild master / jr. master can invite people.

                            return;
                        }

                        string targetName = iPacket.ReadMapleString();

                        Character target = character.Client.Channel.Characters.GetCharacter(targetName);

                        GuildIniviationResult result;

                        if (target == null)
                        {
                            result = GuildIniviationResult.NotInChannel;
                        }
                        else if (target.Guild != null)
                        {
                            result = GuildIniviationResult.AlreadyInGuild;
                        }
                        else
                        {
                            result = GuildIniviationResult.Success;
                        }

                        if (result == GuildIniviationResult.Success)
                        {
                            // TODO: Keep track of invited players so players can't packet edit to join any guild they want.

                            using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                            {
                                oPacket
                                    .WriteByte(5)
                                    .WriteInt(this.ID)
                                    .WriteMapleString(character.Name);

                                target.Client.Send(oPacket);
                            }
                        }
                        else
                        {
                            using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                            {
                                oPacket.WriteByte((byte)result);

                                character.Client.Send(oPacket);
                            }
                        }
                    }
                    break;

                case GuildAction.ModifyNotice:
                    {
                        if (member.Rank > 2)
                        {
                            // NOTE: Only guild master / jr. master can modify the notice.

                            return;
                        }

                        string text = iPacket.ReadMapleString();

                        this.Notice = text;

                        // TODO: Update database or let WorldServer save guilds upon shutdown?

                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                        {
                            oPacket
                                .WriteByte(68)
                                .WriteInt(this.ID)
                                .WriteMapleString(text);

                            this.Broadcast(oPacket);
                        }
                    }
                    break;
            }
        }

        // TODO: Revisit. Resource wasteful as hell.
        public void Broadcast(OutPacket oPacket, GuildMember ignored = null)
        {
            foreach (GuildMember member in this)
            {
                if (member == ignored)
                {
                    continue;
                }

                if (this.World.IsCharacterOnline(member.ID))
                {
                    this.World.GetCharacter(member.ID).Client.Send(oPacket);
                }
            }
        }

        public void Show(Character character)
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
            {
                oPacket
                    .WriteByte(26)
                    .WriteBool(true)
                    .WriteInt(this.ID)
                    .WriteMapleString(this.Name);

                for (int i = 0; i < this.Titles.Length; i++)
                {
                    oPacket.WriteMapleString(this.Titles[i]);
                }

                oPacket.WriteByte((byte)this.Count);

                foreach (GuildMember member in this)
                {
                    oPacket.WriteInt(member.ID);
                }

                foreach (GuildMember member in this)
                {
                    oPacket
                        .WritePaddedString(member.Name, 13)
                        .WriteInt((int)member.Job)
                        .WriteInt(member.Level)
                        .WriteInt(member.Rank)
                        .WriteInt(member.IsOnline ? 1 : 0)
                        .WriteInt() // NOTE: Signature (?).
                        .WriteInt(); // NOTE: Alliance rank.
                }

                oPacket
                    .WriteInt(this.Capacity)
                    .WriteShort(this.Logo)
                    .WriteByte(this.LogoColor)
                    .WriteShort(this.Background)
                    .WriteByte(this.BackgroundColor)
                    .WriteMapleString(this.Notice)
                    .WriteInt(this.Points)
                    .WriteInt(); // NOTE: Alliance ID.

                character.Client.Send(oPacket);
            }
        }

        protected override void InsertItem(int index, GuildMember item)
        {
            item.Guild = this;

            if (MasterServer.IsAlive)
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                {
                    oPacket
                        .WriteByte(39)
                        .WriteInt(this.ID)
                        .WriteInt(item.ID)
                        .WritePaddedString(item.Name, 13)
                        .WriteInt((int)item.Job)
                        .WriteInt(item.Level)
                        .WriteInt(item.Rank)
                        .WriteInt(1)
                        .WriteInt() // NOTE: Signature (?).
                        .WriteInt(); // NOTE: Alliance rank.

                    this.Broadcast(oPacket);
                }
            }

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            GuildMember item = base.Items[index];

            if (MasterServer.IsAlive)
            {
                // TODO: Broadcast leave/expel packet.
            }

            item.Guild = null;

            base.RemoveItem(index);
        }

        protected override int GetKeyForItem(GuildMember item)
        {
            return item.ID;
        }
    }
}
