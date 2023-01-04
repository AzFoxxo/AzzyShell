namespace App.Commands;

using Heroes;

public class History : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1, allowGreaterThanLength: true) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Check no more than 2 args given
        if (args.Length > 2)
        {
            PrintLine("Too many arguments given.");
            return 1;
        }

        // Check if the command is "clear"
        if (args.Length == 2 && args[1] == "clear")
        {
            // Clear the history file
            File.WriteAllText(AzzyShell.GetInstance().historyFile, "");

            // Print success message
            PrintLine("History cleared.");
            
            // Return success
            return 0;
        }

        // Print the history
        var historyFile = AzzyShell.GetInstance().historyFile;

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
        return 0;
    }

    public override string Description => "View the command history or clear it";

    public override void PrintHelp(string[] args)
    {
        PrintLine(Description, Colours.Green);
        PrintLine($"Usage: {this.GetType().Name} [args]", Colours.Green);
        PrintLine("Args:", Colours.Green);
        PrintLine("  arg1 - clear (optional) - Clear the history", Colours.Green);
    }
}