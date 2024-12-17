using System;
using System.Collections.Generic;
using Base;
using UnityEngine;


namespace Factory
{
    public static class CommandFactory
    {
        private static readonly Dictionary<string, Func<CommandData, BaseCommand>> CommandRegistry = 
            new Dictionary<string, Func<CommandData, BaseCommand>>();

        /// <summary>
        /// Registers a command type with the factory.
        /// </summary>
        public static void Register(string commandName, Func<CommandData, BaseCommand> createCommand)
        {
            if (!CommandRegistry.ContainsKey(commandName))
            {
                CommandRegistry[commandName] = createCommand;
            }
            else
            {
                Debug.LogWarning($"Command '{commandName}' is already registered.");
            }
        }

        static CommandFactory()
        {
            ForceInitializeAllCommands();
        }
        
        private static void ForceInitializeAllCommands()
        {
            var assembly = typeof(CommandFactory).Assembly; // or whichever assembly holds your commands
            var commandTypes = assembly.GetTypes();
            foreach (var t in commandTypes)
            {
                if (typeof(BaseCommand).IsAssignableFrom(t) && !t.IsAbstract)
                {
                    // Force the static constructor to run
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(t.TypeHandle);
                }
            }
        }

        /// <summary>
        /// Creates a command instance dynamically based on its name.
        /// </summary>
        public static BaseCommand CreateCommand(CommandData commandData)
        {
            if (CommandRegistry.TryGetValue(commandData.CommandName, out var createCommand))
            {
                return createCommand(commandData);
            }

            throw new Exception($"Unknown command: {commandData.CommandName}");
        }
    }
}