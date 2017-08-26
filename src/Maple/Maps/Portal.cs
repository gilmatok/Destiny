using Destiny.Core.Data;
using Destiny.Core.Network;
using Destiny.Maple.Characters;
using System;

namespace Destiny.Maple.Maps
{
    public class Portal : MapObject
    {
        public byte ID { get; private set; }
        public string Label { get; private set; }
        public int DestinationMapID { get; private set; }
        public string DestinationLabel { get; private set; }
        public string Script { get; private set; }

        public bool IsSpawnPoint
        {
            get
            {
                return this.Label == "sp";
            }
        }

        public Map DestinationMap
        {
            get
            {
                return this.Map.Factory[this.DestinationMapID];
            }
        }

        public Portal Link
        {
            get
            {
                return this.Map.Factory[this.DestinationMapID].Portals[this.DestinationLabel];
            }
        }

        public Portal(Datum datum)
        {
            this.ID = (byte)(int)datum["id"];
            this.Label = (string)datum["label"];
            this.Position = new Point((short)datum["x_pos"], (short)datum["y_pos"]);
            this.DestinationMapID = (int)datum["destination"];
            this.DestinationLabel = (string)datum["destination_label"];
            this.Script = (string)datum["script"];
        }

        public virtual void Enter(Character character)
        {
            Log.Warn("'{0}' attempted to enter an unimplemented portal '{1}'.", character.Name, this.Script);

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.TransferFieldReqInogred))
            {
                oPacket.WriteByte((byte)MapTransferResult.PortalClosed);

                character.Client.Send(oPacket);
            }
        }

        public void PlaySoundEffect(Character character)
        {
            character.ShowLocalUserEffect(UserEffect.PlayPortalSE);
        }

        public void ShowBalloonMessage(Character character, string text, short width, short height)
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.BalloonMsg))
            {
                oPacket
                    .WriteMapleString(text)
                    .WriteShort(width)
                    .WriteShort(height)
                    .WriteByte(1);

                character.Client.Send(oPacket);
            }
        }

        public void ShowTutorialMessage(Character character, string dataPath)
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.Effect))
            {
                oPacket
                    .WriteByte((byte)UserEffect.AvatarOriented)
                    .WriteMapleString(dataPath)
                    .WriteInt(1);

                character.Client.Send(oPacket);
            }
        }
    }
}
