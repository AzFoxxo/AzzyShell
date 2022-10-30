namespace App.Commands;

using Heroes;

public class Vars : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1) != 0) return 2;

        // List all the variables
        Print("Variables:");
        foreach (Variables variable in AzzyShell.GetInstance().variables) {
            Print(variable.name + " - " + variable.type + " - " + variable.value);
        }

        // Return 0 if successful
        return 0;
        
    }
}