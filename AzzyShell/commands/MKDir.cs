namespace App.Commands;

using Heroes;

public class MKDir : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 2) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Make the directory
        Directory.CreateDirectory(args[1]);

        // Return success
        return 0;
    }

    public override string Description => "Create a new directory";

    public override void PrintHelp(string[] args)
    {
        PrintLine(Description, Colours.Green);
        PrintLine($"Usage: {this.GetType().Name} [args]", Colours.Green);
        PrintLine("Args:", Colours.Green);
        PrintLine("  arg1 - Name of directory*", Colours.Green);
    }
}