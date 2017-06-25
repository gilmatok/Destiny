using Destiny.IO;
using System.Net;

namespace Destiny.Server
{
    public sealed class WorldServer
    {
        public string Name { get; private set; }
        public byte Channels { get; private set; }
        public IPAddress HostIP { get; private set; }
        public WorldFlag Flag { get; private set; }
        public string EventMessage { get; private set; }
        public string TickerMessage { get; private set; }
        public int ExperienceRate { get; private set; }
        public int QuestExperienceRate { get; private set; }
        public int PartyQuestExperienceRate { get; private set; }
        public int MesoRate { get; private set; }
        public int DropRate { get; private set; }
        
        public WorldServer()
        {
            this.Name = Settings.GetString("World/Name");
            this.Channels = Settings.GetByte("World/Channels");
            this.HostIP = Settings.GetIPAddress("World/HostIP");
            this.Flag = Settings.GetEnum<WorldFlag>("World/Flag");
            this.EventMessage = Settings.GetString("World/EventMessage");
            this.TickerMessage = Settings.GetString("World/TickerMessage");
            this.ExperienceRate = Settings.GetInt("World/ExperienceRate");
            this.QuestExperienceRate = Settings.GetInt("World/QuestExperienceRate");
            this.PartyQuestExperienceRate = Settings.GetInt("World/PartyQuestExperienceRate");
            this.MesoRate = Settings.GetInt("World/MesoDropRate");
            this.DropRate = Settings.GetInt("World/ItemDropRate");
        }
    }
}
