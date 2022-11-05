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
        Print("Variables:");
        foreach (Variables variable in AzzyShell.GetInstance().variables) {
            // Is string
            bool isString = false;
            if (variable.type == "String") isString = true; else isString = false;
            
            // Print the variable index in the list, the name, the value and the type
            PrintPrompt($"{AzzyShell.GetInstance().variables.IndexOf(variable)}: ", Colours.Yellow);
            PrintPrompt($"{variable.type} ", Colours.DarkRed);
            PrintPrompt($"{variable.name} ", Colours.Green);
            PrintPrompt($"= ", Colours.White);
            if (isString) Print($" \"{variable.value}\"", Colours.Blue); else Print($" {variable.value}", Colours.Blue);
        }

        // Return 0 if successful
        return 0;
        
    }
}