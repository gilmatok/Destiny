using reWZ.WZProperties;
using System;

namespace WZ2BIN
{
    internal static class Extensions
    {
        public static byte GetByte(this WZObject node, string childName, byte def = 0)
        {
            if (node.HasChild(childName))
            {
                try
                {
                    return (byte)node[childName].ValueOrDie<int>();
                }
                catch (InvalidCastException)
                {
                    return byte.Parse(node[childName].ValueOrDie<string>());
                }
            }
            else
            {
                return def;
            }
        }

        public static short GetShort(this WZObject node, string childName, short def = 0)
        {
            if (node.HasChild(childName))
            {
                try
                {
                    return (short)node[childName].ValueOrDie<int>();
                }
                catch (InvalidCastException)
                {
                    return short.Parse(node[childName].ValueOrDie<string>());
                }
            }
            else
            {
                return def;
            }
        }

        public static int GetInt(this WZObject node, string childName, int def = 0)
        {
            if (node.HasChild(childName))
            {
                try
                {
                    return node[childName].ValueOrDie<int>();
                }
                catch (InvalidCastException)
                {
                    return int.Parse(node[childName].ValueOrDie<string>());
                }
            }
            else
            {
                return def;
            }
        }

        public static string GetString(this WZObject node, string childName, string def = "")
        {
            if (node.HasChild(childName))
            {
                return node[childName].ValueOrDie<string>();
            }
            else
            {
                return def;
            }
        }
    }
}
