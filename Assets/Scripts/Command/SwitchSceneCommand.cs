using Base;
using UnityEngine;

namespace Command
{

    public class SwitchSceneParameters : BaseParameters
    {
        public string sceneName;
    }
    
    public class SwitchSceneCommand: BaseCommand
    {
        private readonly SwitchSceneParameters _parameters;
        
        static SwitchSceneCommand()
        {
            RegisterCommand("SwitchScene", data => new SwitchSceneCommand(data));
        }

        public SwitchSceneCommand(CommandData commandData) : base(commandData)
        {
            _parameters = (SwitchSceneParameters)commandData.Parameters;
        }
        
        /// <inheritdoc />
        public override void Execute()
        {
            Debug.Log($"Switching to scene: {_parameters.sceneName}");
        }
    }
}