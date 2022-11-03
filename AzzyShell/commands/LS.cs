namespace App.Commands;

using Heroes;

public class LS : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1, allowGreaterThanLength: true) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Current directory
        var currentDirectory = Directory.GetCurrentDirectory();

        // Get directory provided (if any)
        if (args.Length == 2)
        {
            // Check if directory exists
            if (!Directory.Exists(args[1]))
            {
                Print("Directory does not exist!", Colours.Red);
                return 1;
            }

            // Set current directory to provided directory
            currentDirectory = args[1];
        }
        // Check for too many arguments
        else if (args.Length > 2)
        {
            Print($"Provide only argument (directory) not {args.Length-1}", Colours.Red);
            return 2;
        }

        // List all the directories in the current directory
        foreach (var dir in Directory.GetDirectories(currentDirectory))
        {
            var directory = dir.Replace(currentDirectory, "").Remove(0, 1) + "/";
            PrintPrompt($"{directory}, ", Colours.Blue);
        }

        // New line
        Print("");

        // List all the directories in the current directory
        foreach (var file in Directory.GetFiles(currentDirectory))
        {
            var fileInfo = new FileInfo(file);
            PrintPrompt($"{fileInfo.Name}, ", Colours.Green);
        }

        // New line
        Print("");


        // Return success
        return 0;
    }
}