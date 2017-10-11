using Destiny.IO;
using System;
using System.Windows.Shell;

namespace Destiny.Shell
{
    public static class Shortcuts
    {
        public static void Apply()
        {
            if (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 1)
            {
                JumpList jumpList = new JumpList();

                for (int id = 0; id < Settings.GetInt("Log/JumpLists"); id++)
                {
                    jumpList.JumpItems.Add(new JumpTask()
                    {
                        ApplicationPath = Application.ExecutablePath + "WvsGame.exe",
                        Title = "Launch Channel " + id.ToString(),
                        Arguments = id.ToString()
                    });
                }

                jumpList.Apply();

                if (System.Windows.Application.Current == null)
                {
                    new System.Windows.Application();
                }

                JumpList.SetJumpList(System.Windows.Application.Current, jumpList);
            }
        }
    }
}
