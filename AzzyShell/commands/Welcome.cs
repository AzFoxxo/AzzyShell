namespace App.Commands;

using Heroes;

public class Welcome : Command
{
    public override int Execute(string[] args)
    {
        // Check if no args are given
        if (CheckArgLength(args, 1) != 0) return 2;

        // Get AzzyShell's variable
        var shell = AzzyShell.GetInstance();
        var variables = shell.variables;

        // Welcome <username>!
        Print($"Welcome "); 
        Print(Environment.UserName, Colours.Green); 
        PrintLine("!");

        // You are using <shell> <version> by <author>.
        Print("You are using ");
        GayPrint($"{variables[1].value} ", newline: false);
        Print($"({variables[0].value})", Colours.DarkBlue);
        Print(" by ");
        Print($"{variables[2].value}", Colours.Blue);
        PrintLine(".");

        // Description
        PrintLine(variables[3].value);

        // Basic getting started info
        Print("Type");
        Print(" 'help' ", Colours.Magenta);
        PrintLine("to get started.");

        // Return success
        return 0;
    }
}
