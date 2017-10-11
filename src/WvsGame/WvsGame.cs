using Destiny.Data;
using Destiny.Interoperability;
using Destiny.IO;
using Destiny.Maple.Data;
using Destiny.Network;
using Destiny.Shell;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Destiny
{
    public static class WvsGame
    {
        private static bool isAlive;
        private static byte channelID;
        private static ManualResetEvent AcceptDone = new ManualResetEvent(false);
        public static ManualResetEvent CenterConnectionDone = new ManualResetEvent(false);

        public static TcpListener Listener { get; private set; }
        public static IPEndPoint RemoteEndPoint { get; set; }
        public static CenterServer CenterConnection { get; set; }
        public static GameClients Clients { get; private set; }
        public static int AutoRestartTime { get; set; }

        public static byte WorldID { get; set; }
        public static string WorldName { get; set; }
        public static string TickerMessage { get; set; }
        public static int ExperienceRate { get; set; }
        public static int QuestExperienceRate { get; set; }
        public static int PartyQuestExperienceRate { get; set; }
        public static int MesoRate { get; set; }
        public static int DropRate { get; set; }
        public static bool AllowMultiLeveling { get; set; }

        public static bool IsAlive
        {
            get
            {
                return isAlive;
            }
            set
            {
                isAlive = value;

                if (!value)
                {
                    AcceptDone.Set();
                }
            }
        }

        public static byte ChannelID
        {
            get
            {
                return channelID;
            }
            set
            {
                channelID = value;

                Console.Title = string.Format("WvsGame v.{0}.{1} ({2}-{3}) - MOTD: {4}",
                    Application.MapleVersion,
                    Application.PatchVersion,
                    WvsGame.WorldName,
                    WvsGame.ChannelID,
                    WvsGame.TickerMessage);
            }
        }

        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].ToLower() == "setup" || !File.Exists(Application.ExecutablePath + "WvsGame.ini"))
            {
                WvsGameSetup.Run();
            }

            start:
            WvsGame.Clients = new GameClients();

            Log.Entitle("WvsGame v.{0}.{1}", Application.MapleVersion, Application.PatchVersion);

            try
            {
                Settings.Initialize(Application.ExecutablePath + "WvsGame.ini");

                Database.Test();
                Database.Analyze(true);

                Shortcuts.Apply();

                WvsGame.AutoRestartTime = Settings.GetInt("Server/AutoRestartTime");
                Log.Inform("Automatic restart time set to {0} seconds.", WvsGame.AutoRestartTime);

                DataProvider.Initialize();

                WvsGame.IsAlive = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            if (WvsGame.IsAlive)
            {
                WvsGame.CenterConnectionDone.Reset();

                new Thread(new ThreadStart(CenterServer.Main)).Start();

                WvsGame.CenterConnectionDone.WaitOne();

#if DEBUG
                string linkPath = Path.Combine(Application.ExecutablePath, "LaunchClient.lnk");
                if (File.Exists(linkPath) && WvsGame.WorldID == 0 && WvsGame.ChannelID == 0) //Only for the first WvsGame instance, and only if shortcut exists
                {
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = linkPath;
                    proc.Start();
                }
#endif
            }
            else
            {
                Log.Inform("Could not start server because of errors.");
            }

            while (WvsGame.IsAlive)
            {
                WvsGame.AcceptDone.Reset();

                WvsGame.Listener.BeginAcceptSocket(new AsyncCallback(WvsGame.OnAcceptSocket), null);

                WvsGame.AcceptDone.WaitOne();
            }

            foreach (GameClient client in WvsGame.Clients)
            {
                client.Stop();
            }

            WvsGame.Dispose();

            Log.Warn("Server stopped.");

            if (WvsGame.AutoRestartTime > 0)
            {
                Log.Inform("Attempting auto-restart in {0} seconds.", WvsGame.AutoRestartTime);

                Thread.Sleep(WvsGame.AutoRestartTime * 1000);

                goto start;
            }
            else
            {
                Console.Read();
            }
        }

        public static void Listen()
        {
            WvsGame.Listener = new TcpListener(IPAddress.Any, WvsGame.RemoteEndPoint.Port);
            WvsGame.Listener.Start();
            Log.Inform("Initialized clients listener on {0}.", WvsGame.Listener.LocalEndpoint);
        }

        public static void Stop()
        {
            WvsGame.IsAlive = false;
        }

        private static void OnAcceptSocket(IAsyncResult asyncResult)
        {
            WvsGame.AcceptDone.Set();

            try
            {
                new GameClient(WvsGame.Listener.EndAcceptSocket(asyncResult));
            }
            catch (ObjectDisposedException) { } // TODO: Figure out why this crashes.
        }

        private static void Dispose()
        {
            if (WvsGame.CenterConnection != null)
            {
                WvsGame.CenterConnection.Dispose();
            }

            if (WvsGame.Listener != null)
            {
                WvsGame.Listener.Stop();
            }

            Log.Inform("Server disposed from thread {0}.", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
