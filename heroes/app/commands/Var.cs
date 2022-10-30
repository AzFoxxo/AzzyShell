namespace App.Commands;

using Heroes;

public class Var : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 3) != 0) return 2;

        // Flags for type, value and name
        string name = "";
        string value = "";
        string type = "";

        // Check if the variable name is valid (only contains letters, underscores and numbers and doesn't start with a number)
        if (!System.Text.RegularExpressions.Regex.IsMatch(args[1], @"^[a-zA-Z_][a-zA-Z0-9_]*$")) {
            Print("Invalid variable name");
            return 1;
        }

        // Set the name
        name = args[1];

        // Check if the variable name is already in use
        foreach (Variables var in AzzyShell.GetInstance().variables) {
            if (var.name == args[1]) {
                Print("Variable name already in use");
                return 1;
            }
        }
        // If the variable surrounded by quotes, remove them and set the value to the string between them and set the type to string
        if (args[2].StartsWith("\"") && args[2].EndsWith("\"")) {
            value = args[2].Substring(1, args[2].Length - 2);
            type = "String";
            AzzyShell.GetInstance().variables.Add(new Variables(name, value, type));
        }

        // Check the variable is a number
        else if (int.TryParse(args[2], out int intValue)) {
            value = args[2];
            type = "Int";
            AzzyShell.GetInstance().variables.Add(new Variables(name, value, type));
        }
        // Check the variable is a float
        else if (double.TryParse(args[2], out double floatValue)) {
            value = args[2];
            type = "Float";
            AzzyShell.GetInstance().variables.Add(new Variables(name, value, type));
        }
        // Check the variable is a boolean
        else if (bool.TryParse(args[2], out bool boolValue)) {
            value = args[2];
            type = "Bool";
            AzzyShell.GetInstance().variables.Add(new Variables(name, value, type));
        }
        // Unknown type
        else {
            Print("Unknown type");
            Print("Types: String use double quotes, Int, Float, Bool");
            return 1;
        }

        // Print all variables
        foreach (Variables var in AzzyShell.GetInstance().variables) {
            Print(var.name + " = " + var.value + " (" + var.type + ")");
        }

        // Return 0 if successful
        return 0;
        
    }
}