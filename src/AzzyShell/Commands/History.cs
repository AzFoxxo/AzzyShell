namespace AzzyShell.Commands;

using Heroes;

public class History : Command
{
    public override int Execute(string[] args)
    {
       // Argument length validation
        if (!IsArgumentLengthValid(args, 2))
            return (int)ErrorCode.InvalidArguments;

        // Check if the command is "clear"
        if (args.Length == 2 && args[1] == "clear")
        {
            // Clear the history file
            File.WriteAllText(Shell.historyFile, "");

            // Print success message
            PrintLine("History cleared.");
            
            // Return success
            return (int)ErrorCode.Success;
        }

        // Print the history
        var historyFile = Shell.historyFile;

        // Check if the history file exists in the home directory
        if (!File.Exists(historyFile))
        {
            // Create the history file
            File.Create(historyFile);
        }

        // Read the history file
        string[] history = File.ReadAllLines(historyFile);

        // Print the history with line numbers starting from the bottom of the file
        for (int i = history.Length - 1; i >= 0; i--)
        {
            PrintLine((history.Length - i) + ": " + history[i]);
        }

        // Return success
        return (int)ErrorCode.Success;
    }
}