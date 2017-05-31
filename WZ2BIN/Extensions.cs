using reWZ.WZProperties;
using System;

namespace WZ2BIN
{
    internal static class Extensions
    {
        public static int GetID(this WZObject node)
        {
            return int.Parse(node.Name.Replace(".img", ""));
        }

        public static int GetInt(this WZObject parentNode, string childName, int def = 0)
        {
            if (!parentNode.HasChild(childName))
            {
                return def;
            }

            WZObject childNode = parentNode[childName];

            if (childNode is WZInt32Property)
                return (childNode as WZInt32Property).Value;
            if (childNode is WZStringProperty)
                return int.Parse((childNode as WZStringProperty).Value);

            throw new Exception("unablet o cast");
        }

        public static double GetDouble(this WZObject node, string childName, double def = 0)
        {
            return node.HasChild(childName) ? node[childName].ValueOrDie<double>() : def;
        }

        public static string GetString(this WZObject node, string childName, string def = "")
        {
            return node.HasChild(childName) ? node[childName].ValueOrDie<string>() : def;
        }
    }
}
