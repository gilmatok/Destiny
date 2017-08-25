using Destiny.Core.Data;
using Destiny.Core.Network;
using Destiny.Maple.Characters;

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

            character.Release();
        }

        public void PlaySoundEffect(Character character)
        {
            character.ShowLocalUserEffect(UserEffect.PlayPortalSE);
        }
    }
}
