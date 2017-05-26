namespace Destiny.Server
{
    public sealed class ChannelServer : ServerBase
    {
        public byte ID { get; private set; }
        public byte WorldID { get; private set; }

        public ChannelServer(byte id, byte worldID, short port) 
            : base("Channel", port)
        {
            this.ID = id;
            this.WorldID = worldID;
        }

        protected override void RegisterHandlers()
        {

        }
    }
}
