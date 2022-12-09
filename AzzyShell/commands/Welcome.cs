namespace App.Commands;

using Heroes;

public class Welcome : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Print welcome message
        Print($"Welcome "); Print(Environment.UserName, Colours.Green); PrintLine("!");
        Print("You are using "); GayPrint($"{AzzyShell.GetInstance().variables[1].value} ", newline: false); Print($"({AzzyShell.GetInstance().variables[0].value})", Colours.DarkBlue); Print(" by "); Print($"{AzzyShell.GetInstance().variables[2].value}", Colours.Blue); PrintLine(".");
        PrintLine(AzzyShell.GetInstance().variables[3].value);
        Print("Type"); Print(" 'help' ", Colours.Magenta); PrintLine("to get started.");

        // Return success
        return 0;
    }
}