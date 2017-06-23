using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Destiny.Threading
{
    public sealed class Delay
    {
        public static void Execute(double delay, ThreadStart action)
        {
            Delay.Execute((int)delay, action);
        }

        public static void Execute(int delay, ThreadStart action)
        {
            Timer t = new Timer(delay);

            t.Elapsed += new ElapsedEventHandler(delegate (object sender, ElapsedEventArgs e)
            {
                t.Stop();
                action();
                t.Dispose();
                t = null;
            });

            t.Start();
        }

        private ThreadStart action;
        private Timer t;

        public Delay(int delay, ThreadStart action)
        {
            t = new Timer(delay);

            t.Elapsed += new ElapsedEventHandler(delegate (object sender, ElapsedEventArgs e)
            {
                if (t != null)
                {
                    t.Stop();
                    action();
                }

                t = null;
            });
        }

        public void Execute()
        {
            t.Start();
        }

        public void Cancel()
        {
            if (t != null)
            {
                t.Stop();
                t.Dispose();
            }

            t = null;
        }
    }
}