using System.Collections.Generic;

namespace Base
{
    public class CommandsRoot
    {
        public List<CommandData> Commands;  // Must match "Commands" in JSON (case-insensitive)
    }

    public class CommandData
    {
        public string CommandName;          // Retrieves "CommandName" (case-insensitive)
        public BaseParameters Parameters;   // Abstract field
    }
}