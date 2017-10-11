using Destiny.IO;
using Destiny.Maple.Characters;
using System;
using System.Reflection;

namespace Destiny.Maple.Commands
{
    public static class CommandFactory
    {
        public static Commands Commands { get; private set; }

        public static void Initialize()
        {
            CommandFactory.Commands = new Commands();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(Command)))
                {
                    CommandFactory.Commands.Add((Command)Activator.CreateInstance(type));
                }
            }
        }

        public static void Execute(Character caller, string text)
        {
            string[] splitted = text.Split(' ');

            splitted[0] = splitted[0].ToLower();

            string commandName = splitted[0].TrimStart(Application.CommandIndiciator);

            string[] args = new string[splitted.Length - 1];

            for (int i = 1; i < splitted.Length; i++)
            {
                args[i - 1] = splitted[i];
            }

            if (CommandFactory.Commands.Contains(commandName))
            {
                Command command = CommandFactory.Commands[commandName];

                if (!command.IsRestricted || caller.IsMaster)
                {
                    try
                    {
                        command.Execute(caller, args);
                    }
                    catch (Exception e)
                    {
                        caller.Notify("[Command] Unknown error: " + e.Message);
                        Log.Error("{0} error by {1}: ", e, command.GetType().Name, caller.Name);
                    }
                }
                else
                {
                    caller.Notify("[Command] Restricted command.");
                }
            }
            else
            {
                caller.Notify("[Command] Invalid command.");
            }
        }
    }
}
