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
                PrintLine("Directory does not exist!", Colours.Red);
                return 1;
            }

            // Set current directory to provided directory
            currentDirectory = args[1];
        }
        // Check for too many arguments
        else if (args.Length > 2)
        {
            PrintLine($"Provide only argument (directory) not {args.Length-1}", Colours.Red);
            return 2;
        }

        // List all the directories in the current directory
        foreach (var dir in Directory.GetDirectories(currentDirectory))
        {
            // Print the directory name
            Print(Path.GetFileName(dir) + ", ", Colours.Blue);
        }

        // New line
        PrintLine("");

        // List all the directories in the current directory
        foreach (var file in Directory.GetFiles(currentDirectory))
        {
            var fileInfo = new FileInfo(file);
            Print($"{fileInfo.Name}, ", Colours.Green);
        }

        // New line
        PrintLine("");


        // Return success
        return 0;
    }

    public override string Description => "List all files and directories in the current directory";

    public override void PrintHelp(string[] args)
    {
        PrintLine(Description, Colours.Green);
        PrintLine($"Usage: {this.GetType().Name} [args]", Colours.Green);
        PrintLine("Args:", Colours.Green);
        PrintLine("  arg1 - Directory to list (optional)", Colours.Green);
    }
}