namespace App.Commands;

using Heroes;

public class CommandTemplate : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 2) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Command logic
        PrintLine("Your new command is: ");
        Print(@"namespace App.Commands;

using Heroes;

public class ", Colours.Green); Print(args[1], Colours.Magenta); PrintLine(@" : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Command logic

        // Return success
        return 0;
    }

    public override string Description => ""Description of the command (used in help)"";

    public override void PrintHelp(string[] args)
    {
        PrintLine(Description, Colours.Green);
        PrintLine($""Usage: {this.GetType().Name} [args]"", Colours.Green);
        PrintLine(""Args:"", Colours.Green);
        PrintLine(""  arg1 - Description of arg1"", Colours.Green);
    }
}", Colours.Green);

        // Return success
        return 0;
    }

    public override string Description => "Generate C# code for a new command";

    public override void PrintHelp(string[] args)
    {
        PrintLine(Description, Colours.Green);
        PrintLine($"Usage: {this.GetType().Name} [args]", Colours.Green);
        PrintLine("Args:", Colours.Green);
        PrintLine("  arg1 - Name of command*", Colours.Green);
    }

}