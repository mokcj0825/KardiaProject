using System;
using Factory;

namespace Base
{
    [System.Serializable]
    public abstract class BaseCommand
    {
        protected CommandData CommandData;
        
        protected BaseCommand(CommandData commandData)
        {
            CommandData = commandData;
        }
        
        public string CommandName;
        
        public abstract void Execute();
        
        protected static void RegisterCommand(string commandName, Func<CommandData, BaseCommand> createCommand)
        {
            CommandFactory.Register(commandName, createCommand);
        }
        
    }
}