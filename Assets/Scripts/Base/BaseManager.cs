using System.Collections.Generic;
using Factory;
using Object;
using UnityEngine;
using Utility;

namespace Base
{
    /// <summary>
    /// A general-purpose manager that loads and executes commands.
    /// </summary>
    public abstract class BaseManager : MonoBehaviour
    {
        private Queue<BaseCommand> _commandQueue; // Command execution queue
        private GameContext _context;

        /// <summary>
        /// Unity Start method. Initializes the manager and starts execution.
        /// </summary>
        protected virtual void Start()
        {
            // Initialize game context
            _context = InitializeGameContext();

            // Load commands
            var commands = LoadCommands();

            // Initialize the command queue
            _commandQueue = new Queue<BaseCommand>(commands);

            // Start command execution
            ExecuteCommands();
        }
        
        /// <summary>
        /// Initializes the game context. Override this to customize the context.
        /// </summary>
        /// <returns>An instance of the game context.</returns>
        protected virtual GameContext InitializeGameContext()
        {
            return GameContext.Instance;
        }
        
        /// <summary>
        /// Loads commands for the manager. Must be implemented by derived classes.
        /// </summary>
        /// <returns>A list of commands to execute.</returns>
        protected abstract List<BaseCommand> LoadCommands();
        
        /// <summary>
        /// Parses the JSON text into a list of CommandData objects.
        /// </summary>
        /// <param name="json">The JSON string to parse.</param>
        /// <returns>A list of CommandData objects.</returns>
        protected virtual List<BaseCommand> ParseCommandData(string json)
        {
            var wrapper = CustomJsonUtility.FromJson<CommandDataWrapper>(json);

            if (wrapper == null || wrapper.Commands == null)
            {
                throw new System.Exception("Failed to parse commands: Invalid JSON or missing 'Commands' array.");
            }

            var commands = new List<BaseCommand>();

            foreach (var commandData in wrapper.Commands)
            {
                commands.Add(CommandFactory.CreateCommand(commandData));
            }

            return commands;
        }

        /// <summary>
        /// Starts executing commands. Override this to customize execution flow.
        /// </summary>
        protected virtual void ExecuteCommands()
        {
            StartCoroutine(ExecuteAllCommands());
        }

        /// <summary>
        /// Executes all commands in the queue.
        /// </summary>
        private System.Collections.IEnumerator ExecuteAllCommands()
        {
            while (_commandQueue.Count > 0)
            {
                var command = _commandQueue.Dequeue();
                Debug.Log($"Executing command: {command.CommandName}");
                command.Execute();
                yield return null; // Wait for the next frame
            }

            OnExecutionComplete();
        }

        /// <summary>
        /// Called when all commands have been executed.
        /// </summary>
        protected virtual void OnExecutionComplete()
        {
            Debug.Log("All commands executed.");
        }
        
        /// <summary>
        /// Wrapper class for parsing CommandData from JSON.
        /// </summary>
        [System.Serializable]
        protected class CommandDataWrapper
        {
            public List<CommandData> Commands;
        }
        
        
    }
    
    
}