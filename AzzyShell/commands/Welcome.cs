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
        PrintPrompt($"Welcome "); PrintPrompt(Environment.UserName, Colours.Green); Print("!");
        PrintPrompt("You are using "); GayPrint($"{AzzyShell.GetInstance().variables[1].value} ", newline: false); PrintPrompt($"({AzzyShell.GetInstance().variables[0].value})", Colours.DarkBlue); PrintPrompt(" by "); PrintPrompt($"{AzzyShell.GetInstance().variables[2].value}", Colours.Blue); Print(".");
        Print(AzzyShell.GetInstance().variables[3].value);
        PrintPrompt("Type"); PrintPrompt(" 'help' ", Colours.Magenta); Print("to get started.");

        // Return success
        return 0;
    }
}