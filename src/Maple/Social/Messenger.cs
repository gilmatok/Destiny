using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Social
{
    public sealed class Messenger
    {
        public int ID { get; private set; }
        public Character[] Participants { get; private set; }

        public int Count
        {
            get
            {
                int count = 0;

                for (int i = 0; i < this.Participants.Length; i++)
                {
                    if (this.Participants[i] != null)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public bool IsFull
        {
            get
            {
                return this.Count == 3;
            }
        }

        public Messenger(int id, Character host)
        {
            this.ID = id;
            this.Participants = new Character[3];

            this.AddParticipant(host, true);
        }

        public void AddParticipant(Character participant, bool isFirst = false)
        {
            participant.Messenger = this;

            byte index = 0;

            for (int i = 0; i < this.Participants.Length; i++)
            {
                if (this.Participants[i] == null)
                {
                    index = (byte)i;

                    this.Participants[i] = participant;

                    break;
                }
            }

            if (!isFirst)
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.Messenger))
                {
                    oPacket
                        .WriteByte((byte)MessengerResult.Join)
                        .WriteByte(index);

                    participant.Client.Send(oPacket);
                }

                for (int i = 0; i < this.Participants.Length; i++)
                {
                    if (this.Participants[i] != null)
                    {
                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.Messenger))
                        {
                            oPacket
                                .WriteByte((byte)MessengerResult.Open)
                                .WriteByte(index)
                                .WriteBytes(participant.AppearanceToByteArray())
                                .WriteMapleString(participant.Name)
                                .WriteByte(participant.Client.ChannelID)
                                .WriteByte();

                            this.Participants[i].Client.Send(oPacket);
                        }

                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.Messenger))
                        {
                            oPacket
                                .WriteByte((byte)MessengerResult.Open)
                                .WriteByte((byte)i)
                                .WriteBytes(this.Participants[i].AppearanceToByteArray())
                                .WriteMapleString(this.Participants[i].Name)
                                .WriteByte(this.Participants[i].Client.ChannelID)
                                .WriteByte();

                            participant.Client.Send(oPacket);
                        }
                    }
                }
            }
        }

        public void RemoveParticipant(Character participant)
        {
            byte index = 0;

            for (int i = 0; i < this.Participants.Length; i++)
            {
                if (this.Participants[i] != null)
                {
                    if (this.Participants[i] == participant)
                    {
                        index = (byte)i;

                        this.Participants[i] = null;

                        break;
                    }
                }
            }

            participant.Messenger = null;

            if (this.Count == 0)
            {
                participant.Client.World.RemoveMessenger(this.ID);
            }
            else
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.Messenger))
                {
                    oPacket
                        .WriteByte((byte)MessengerResult.Leave)
                        .WriteByte(index);

                    this.Broadcast(oPacket);
                }
            }
        }

        public void Broadcast(OutPacket oPacket, Character ignored = null)
        {
            for (int i = 0; i < this.Participants.Length; i++)
            {
                if (this.Participants[i] != null)
                {
                    if (this.Participants[i] != ignored)
                    {
                        this.Participants[i].Client.Send(oPacket);
                    }
                }
            }
        }
    }
}
