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
            
            // Print the variable name, type and value
            Print($"Name: {variable.name} Type: {variable.type} Value: {(isString ? "\"" : "")}{variable.value}{(isString ? "\"" : "")}");
        }

        // Return 0 if successful
        return 0;
        
    }
}