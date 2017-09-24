using Destiny.Maple.Characters;
using Destiny.Scripting;
using System;

namespace Destiny.Maple.Scripting
{
    public abstract class ScriptBase : LuaScriptable
    {
        private Character mCharacter;

        protected ScriptBase(ScriptType type, string name, Character character, bool useThread)
            : base(string.Format(Application.ExecutablePath + @"..\..\scripts\{0}\{1}.lua", type.ToString().ToLower(), name), useThread)
        {
            this.Expose("test", new Action<string>((string text) => { Log.Inform("Script test: {0}", text); }));
        }
    }
}
