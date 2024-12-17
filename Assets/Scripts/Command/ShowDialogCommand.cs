using Base;
using UnityEngine;

namespace Command
{
    
    public class ShowDialogParameters : BaseParameters
    {
        public string character;
        public string text;
        public string position;
    }
    
    [System.Serializable]
    public class ShowDialogCommand : BaseCommand
    {
        private readonly ShowDialogParameters _parameters;
        
        static ShowDialogCommand()
        {
            RegisterCommand("ShowDialog", data => new ShowDialogCommand(data));
        }
        
        public ShowDialogCommand(CommandData commandData) : base(commandData)
        {
            _parameters = (ShowDialogParameters)commandData.Parameters;
        }
        
        public override void Execute()
        {
            Debug.Log($"ShowDialog: {_parameters.character} says {_parameters.text} at {_parameters.position}");
            //context.DialogSystem.Show(Character, Text, Position);
        }
    }
}