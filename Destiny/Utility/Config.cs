using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Destiny.Utility
{
    public sealed class Config
    {
        public static Config Instance { get; private set; }
        
        public static void Load(string path = null)
        {
            if (path == null)
            {
                path = "Config.xml";
            }

            using (XmlReader reader = XmlReader.Create(path))
            {
                Config.Instance = (Config)(new XmlSerializer(typeof(Config))).Deserialize(reader);
            }

            Logger.Write(LogLevel.Info, "Configuration file '{0}' loaded.", Path.GetFileName(path));
        }

        public string Binary { get; set; }
        public CDatabase Database { get; set; }
        public CLogin Login { get; set; }
        public List<CWorld> Worlds { get; set; }
    }

    public class CDatabase
    {
        public string Host { get; set; }
        public string Schema { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class CLogin
    {
        public short Port { get; set; }
        public bool AutoRegister { get; set; }
        public bool RequestPin { get; set; }
        public bool RequestPic { get; set; }
    }

    public class CWorld
    {
        public byte ID { get; set; }
        public string Name { get; set; }
        public short Port { get; set; }
        public byte Channels { get; set; }
        public WorldFlag Flag { get; set; }
        public string EventMessage { get; set; }
        public string TickerMessage { get; set; }
    }
}
