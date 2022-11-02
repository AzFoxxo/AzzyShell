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
        Print($"Welcome {System.Environment.UserName}!");
        Print($"You are using {AzzyShell.GetInstance().variables[1].value} version {AzzyShell.GetInstance().variables[0].value} by {AzzyShell.GetInstance().variables[2].value}.");
        Print(AzzyShell.GetInstance().variables[3].value);
        Print("Type 'help' to get started.");

        // Return success
        return 0;
    }
}