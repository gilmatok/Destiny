using Destiny.IO;
using MoonSharp.Interpreter;
using System.IO;
using System.Threading;

namespace Destiny.Scripting
{
    public abstract class LuaScriptable
    {
        private string mPath;
        private Script mScript;
        private bool mUseThread;

        protected LuaScriptable(string path, bool useThread = false)
        {
            mPath = Path.GetFullPath(path);
            mScript = new Script();
            mUseThread = useThread;
        }

        public void Execute()
        {
			if (!File.Exists(mPath))
			{
				var dirInfo = new DirectoryInfo(mPath);
				Log.Warn("Unimplemented Script: {0}", Path.Combine(dirInfo.Parent.Parent.Name, dirInfo.Parent.Name, dirInfo.Name)); // Looks like this: scripts/npc/1234567.lua
				return;
			}

			if (mUseThread)
			{
				new Thread(new ThreadStart(() => mScript.DoFile(mPath))).Start();
			}
			else
			{
				mScript.DoFile(mPath);
			}
        }

        protected void Expose(string key, object value)
        {
            mScript.Globals[key] = value;
        }
    }
}
