using MoonSharp.Interpreter;
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
            mPath = path;
            mScript = new Script();
            mUseThread = useThread;
        }

        public void Execute()
        {
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
