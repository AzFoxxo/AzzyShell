namespace App.Commands;

using Heroes;

public class GayText : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 2, true) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Join all the args together with a space in between (excluding the first arg)
        string message = string.Join(" ", args, 1, args.Length - 1);

        // Print the message in gay colours
        GayPrint(message);

        // Return success
        return 0;
    }

    public override string Description => "Print a message in gay text";

    public override void PrintHelp(string[] args)
    {
        PrintLine(Description, Colours.Green);
        PrintLine($"Usage: {this.GetType().Name} [args]", Colours.Green);
        PrintLine("Args:", Colours.Green);
        PrintLine("  arg1 - Message to print*", Colours.Green);
    }
}