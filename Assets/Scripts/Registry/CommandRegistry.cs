using System;
using System.Collections.Generic;
using Command;

namespace Registry
{
    public static class CommandRegistry
    {
        private static readonly Dictionary<string, Type> CommandMappings = new Dictionary<string, Type>();

        static CommandRegistry()
        {
            // Registering command types to their respective parameter types dynamically
            RegisterCommandType("ShowDialog", typeof(ShowDialogParameters));
            RegisterCommandType("SwitchScene", typeof(SwitchSceneParameters));
        }
        
        // Register the command type to its respective parameter type
        public static void RegisterCommandType(string commandType, Type parameterType)
        {
            CommandMappings[commandType] = parameterType;
        }

        // Resolve the parameter type for the command
        public static Type ResolveParameterType(string commandType)
        {
            if (CommandMappings.ContainsKey(commandType))
            {
                return CommandMappings[commandType];
            }

            throw new Exception($"Unknown command type: {commandType}");
        }
    }
}