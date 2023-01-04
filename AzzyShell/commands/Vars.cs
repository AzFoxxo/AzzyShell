namespace App.Commands;

using Heroes;

public class Vars : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // List all the variables
        foreach (Variables variable in AzzyShell.GetInstance().variables) {
            // Is string
            bool isString = false;
            if (variable.type == "String") isString = true; else isString = false;
            
            // Print the variable index in the list, the name, the value and the type
            Print($"{AzzyShell.GetInstance().variables.IndexOf(variable)}: ", Colours.Yellow);
            Print($"{variable.type} ", Colours.DarkRed);
            Print($"{variable.name} ", Colours.Green);
            Print($"= ", Colours.White);
            if (isString) PrintLine($" \"{variable.value}\"", Colours.Blue); else PrintLine($" {variable.value}", Colours.Blue);
        }

        // Return 0 if successful
        return 0;
        
    }

    public override string Description => "List all variables";

    public override void PrintHelp(string[] args)
    {
        PrintLine(Description, Colours.Green);
        PrintLine($"Usage: {this.GetType().Name}", Colours.Green);
    }
}