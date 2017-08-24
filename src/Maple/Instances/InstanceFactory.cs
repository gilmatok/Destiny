using Destiny.Server;
using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Destiny.Maple.Instances
{
    public sealed class InstanceFactory : KeyedCollection<string, Instance>
    {
        public ChannelServer Channel { get; private set; }

        public InstanceFactory(ChannelServer channel)
            : base()
        {
            this.Channel = channel;
        }

        public bool Create(string name, int time)
        {
            Type implementedType = Assembly.GetExecutingAssembly().GetType("Destiny.Maple.Instances.Implementation." + name);

            if (implementedType == null)
            {
                return false;
            }

            this.Add((Instance)Activator.CreateInstance(implementedType, this.Channel, time));

            return true;
        }

        protected override string GetKeyForItem(Instance item)
        {
            return item.Name;
        }
    }
}
