using Destiny.Core.Data;
using Destiny.Core.IO;
using Destiny.Maple;
using Destiny.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Destiny
{
    public static class WvsCenter
    {
        private static bool isAlive;
        private static ManualResetEvent AcceptDone = new ManualResetEvent(false);

        public static CenterClient Login { get; set; }
        public static Worlds Worlds { get; private set; }
        public static Migrations Migrations { get; private set; }

        public static TcpListener Listener { get; private set; }
        public static List<CenterClient> Clients { get; private set; }

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
                    WvsCenter.AcceptDone.Set();
                }
            }
        }

        private static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].ToLower() == "setup" || !File.Exists(Application.ExecutablePath + "WvsCenter.ini"))
            {
                WvsCenterSetup.Run();
            }

            WvsCenter.Worlds = new Worlds();
            WvsCenter.Migrations = new Migrations();
            WvsCenter.Clients = new List<CenterClient>();

            Log.Entitle("WvsCenter v.{0}.{1}", Application.MapleVersion, Application.PatchVersion);

            try
            {
                Settings.Initialize(Application.ExecutablePath + "WvsCenter.ini");

                Database.Test();
                Database.Analyze(false);

                CenterClient.SecurityCode = Settings.GetString("Server/SecurityCode");
                Log.Inform("Cross-servers code '{0}' assigned.", Log.MaskString(CenterClient.SecurityCode));

                WvsCenter.Listener = new TcpListener(IPAddress.Any, Settings.GetInt("Server/Port"));
                WvsCenter.Listener.Start();
                Log.Inform("Initialized clients listener on {0}.", WvsCenter.Listener.LocalEndpoint);

                WvsCenter.IsAlive = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }


            if (WvsCenter.IsAlive)
            {
                Log.Success("WvsCenter started on thread {0}.", Thread.CurrentThread.ManagedThreadId);
            }
            else
            {
                Log.Inform("Could not start server because of errors.");
            }

            while (WvsCenter.IsAlive)
            {
                WvsCenter.AcceptDone.Reset();

                WvsCenter.Listener.BeginAcceptSocket(new AsyncCallback(WvsCenter.OnAcceptSocket), null);

                WvsCenter.AcceptDone.WaitOne();
            }

            CenterClient[] remainingServers = WvsCenter.Clients.ToArray();

            foreach (CenterClient server in remainingServers)
            {
                server.Stop();
            }

            WvsCenter.Dispose();

            Log.Warn("Server stopped.");

            Console.Read();
        }

        private static void OnAcceptSocket(IAsyncResult ar)
        {
            WvsCenter.AcceptDone.Set();

            new CenterClient(WvsCenter.Listener.EndAcceptSocket(ar));
        }

        public static void Stop()
        {
            WvsCenter.IsAlive = false;
        }

        private static void Dispose()
        {
            if (WvsCenter.Listener != null)
            {
                WvsCenter.Listener.Stop();
            }

            Log.Inform("Server disposed from thread {0}.", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
