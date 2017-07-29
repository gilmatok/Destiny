using Destiny.Maple.Characters;
using Jint;
using System;
using System.IO;

namespace Destiny.Maple.Script
{
    public abstract class ScriptBase
    {
        private string mPath;
        private Character mCharacter;
        private Engine mEngine;

        protected ScriptBase(string path, Character character)
        {
            mPath = path;
            mCharacter = character;
            mEngine = new Engine();

            this.Expose("changeMap", new Action<int, byte>(this.ChangeMap));
        }

        public void Execute()
        {
            string contents = File.ReadAllText(mPath);

            mEngine.Execute(contents);
        }

        protected void Expose(string name, object value)
        {
            mEngine.SetValue(name, value);
        }

        private void ChangeMap(int mapID, byte portalID = 0)
        {
            try
            {
                mCharacter.ChangeMap(mapID, portalID);
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
