using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Social
{
    public sealed class Messenger
    {
        public int ID { get; private set; }
        public Character[] Participants { get; private set; }

        public bool IsFull
        {
            get
            {
                bool ret = true;

                for (int i = 0; i < this.Participants.Length; i++)
                {
                    if (this.Participants[i] == null)
                    {
                        ret = false;
                    }
                }

                return ret;
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

        }

        public void Broadcast(OutPacket oPacket)
        {
            for (int i = 0; i < this.Participants.Length; i++)
            {
                if (this.Participants[i] != null)
                {
                    this.Participants[i].Client.Send(oPacket);
                }
            }
        }
    }
}
