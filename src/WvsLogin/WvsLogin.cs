using Destiny.Data;
using Destiny.Interoperability;
using Destiny.IO;
using Destiny.Maple;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Destiny.Network
{
    public static class WvsLogin
    {
        private static bool isAlive;
        private static ManualResetEvent AcceptDone = new ManualResetEvent(false);
        public static ManualResetEvent CenterConnectionDone = new ManualResetEvent(false);

        public static TcpListener Listener { get; private set; }
        public static CenterServer CenterConnection { get; set; }
        public static Worlds Worlds { get; private set; }
        public static LoginClients Clients { get; private set; }

        public static bool AutoRegister { get; private set; }
        public static bool RequestPin { get; private set; }
        public static bool RequestPic { get; private set; }
        public static bool RequireStaffIP { get; private set; }
        public static int MaxCharacters { get; private set; }

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
                    WvsLogin.AcceptDone.Set();
                }
            }
        }

        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].ToLower() == "setup" || !File.Exists(Application.ExecutablePath + "WvsLogin.ini"))
            {
                WvsLoginSetup.Run();
            }

            WvsLogin.Worlds = new Worlds();
            WvsLogin.Clients = new LoginClients();

            Log.Entitle("Destiny - Login Server v.{0}.{1}", Application.MapleVersion, Application.PatchVersion);

            try
            {
                Settings.Initialize(Application.ExecutablePath + "WvsLogin.ini");

                Database.Test();
                Database.Analyze(false);

                WvsLogin.RequireStaffIP = Settings.GetBool("Server/RequireStaffIP");
                Log.Inform("Staff will{0}be required to connect through a staff IP.", WvsLogin.RequireStaffIP ? " " : " not ");

                WvsLogin.AutoRegister = Settings.GetBool("Server/AutoRegister");
                Log.Inform("Automatic registration {0}.", WvsLogin.AutoRegister ? "enabled" : "disabled");

                WvsLogin.RequestPin = Settings.GetBool("Server/RequestPin");
                Log.Inform("Pin will{0}be requested upon login.", WvsLogin.RequestPin ? " " : " not ");

                WvsLogin.RequestPic = Settings.GetBool("Server/RequestPic");
                Log.Inform("Pic will{0}be requested upon character selection.", WvsLogin.RequestPic ? " " : " not ");

                WvsLogin.MaxCharacters = Settings.GetInt("Server/MaxCharacters");
                Log.Inform("Maximum of {0} characters per account.", WvsLogin.MaxCharacters);

                for (byte i = 0; i < Settings.GetByte("Server/Worlds"); i++)
                {
                    WvsLogin.Worlds.Add(new World(i));
                }

                WvsLogin.isAlive = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            if (WvsLogin.IsAlive)
            {
                WvsLogin.CenterConnectionDone.Reset();

                new Thread(new ThreadStart(CenterServer.Main)).Start();

                WvsLogin.CenterConnectionDone.WaitOne();
            }
            else
            {
                Log.Inform("Could not start server because of errors.");
            }

            while (WvsLogin.IsAlive)
            {
                WvsLogin.AcceptDone.Reset();

                WvsLogin.Listener.BeginAcceptSocket(new AsyncCallback(WvsLogin.OnAcceptSocket), null);

                WvsLogin.AcceptDone.WaitOne();
            }

            foreach (LoginClient client in WvsLogin.Clients)
            {
                client.Stop();
            }

            WvsLogin.Dispose();

            Log.Warn("Server stopped.");

            Console.Read();
        }

        public static void Listen()
        {
            WvsLogin.Listener = new TcpListener(IPAddress.Any, Settings.GetInt("Server/Port"));
            WvsLogin.Listener.Start();
            Log.Inform("Initialized clients listener on {0}.", WvsLogin.Listener.LocalEndpoint);
        }

        private static void OnAcceptSocket(IAsyncResult ar)
        {
            WvsLogin.AcceptDone.Set();

            try
            {
                new LoginClient(WvsLogin.Listener.EndAcceptSocket(ar));
            }
            catch (ObjectDisposedException) { } // TODO: Figure out why this crashes.
        }

        public static void Stop()
        {
            WvsLogin.IsAlive = false;
        }

        private static void Dispose()
        {
            if (WvsLogin.Listener != null)
            {
                WvsLogin.Listener.Stop();
            }

            Log.Inform("Server disposed.", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
