namespace App.Commands;

using Heroes;

public class CD : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 2) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Check if the directory exists, if it does, change to it
        if (Directory.Exists(args[1]))
        {
            Directory.SetCurrentDirectory(args[1]);
        }
        else if (Directory.Exists(Directory.GetCurrentDirectory() + "/" + args[1]))
        {
            Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "/" + args[1]);
        }
        else
        {
            PrintLine($"Directory does not exist {args[1]}");
            return 1;
        }

        // Return success
        return 0;
    }

    public override string Description => "Change the current directory";

    public override void PrintHelp(string[] args)
    {
        PrintLine(Description, Colours.Green);
        PrintLine($"Usage: {this.GetType().Name} [args]", Colours.Green);
        PrintLine("Args:", Colours.Green);
        PrintLine("  arg1 - Name of directory*", Colours.Green);
    }
}