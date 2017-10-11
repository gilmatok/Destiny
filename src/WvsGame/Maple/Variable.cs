using Destiny.Data;
using Destiny.Data;

namespace Destiny.Maple
{
    public sealed class Variable
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public Variable(string key, object value)
        {
            this.Key = key;
            this.Value = value.ToString();
        }

        public Variable(Datum datum)
        {
            this.Key = (string)datum["key"];
            this.Value = (string)datum["Value"];
        }
    }
}
