using System.Collections.Generic;
using Base;
using UnityEngine;

namespace Examples
{
    public class ExampleManager : BaseManager
    {
        
        protected override List<BaseCommand> LoadCommands()
        {
            // Example: Load commands from a file or other source
            var textAsset = Resources.Load<TextAsset>("ExampleScript001"); // From Resources/ExampleScript001.json
            if (textAsset == null)
            {
                throw new System.IO.FileNotFoundException("Dialog file not found.");
            }

            // Parse the JSON data into commands (specific logic here)
            var commandDataList = ParseCommandData(textAsset.text);
            
            return commandDataList;
        }
       

        protected override void OnExecutionComplete()
        {
            Debug.Log("Scene execution complete");
        }
    }
}