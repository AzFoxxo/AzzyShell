namespace App.Commands;

using Heroes;

public class Set : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 3) != 0) return 2;

        // Flags for type, value and name
        string name = args[1];
        string value = args[2];
        string type = "";

        // Check if the variable name exists
        bool found = false;
        foreach (Variables variable in AzzyShell.GetInstance().variables) {
            if (variable.name == name) {
                found = true;
                break;
            }
        }

        // If the variable name doesn't exist, return an error
        if (!found) {
            Print("Variable name doesn't exist, reassign of non-existent variable not allowed!");
            return 1;
        }

        // If the variable surrounded by quotes, remove them and set the value to the string between them and set the type to string
        if (value.StartsWith("\"") && value.EndsWith("\"")) {
            value = value.Substring(1, value.Length - 2);
            type = "String";
        }

        // Check the variable is a number
        else if (int.TryParse(value, out int intValue)) {
            type = "Int";
        }
        
        // Check the variable is a double
        else if (double.TryParse(value, out double doubleValue)) {
            type = "Double";
        }

        // Check the variable is a boolean
        else if (bool.TryParse(value, out bool boolValue)) {
            type = "Bool";
        }

        // Unknown type
        else {
            Print("Unknown type");
            Print("Types: String use double quotes, Int, Double, Bool");
            return 1;
        }

        // Create an array copy of the variables list
        Variables[] variablesArrayCopy = AzzyShell.GetInstance().variables.ToArray();

        // Get the index of the variable to be reassigned
        int index = 0;
        for (int i = 0; i < variablesArrayCopy.Length; i++) {
            if (variablesArrayCopy[i].name == name) {
                index = i;
                break;
            }
        }

        // Reassign the variable
        variablesArrayCopy[index] = new Variables(name, value, type);

        // Set the variables list to the array copy
        AzzyShell.GetInstance().variables = new List<Variables>(variablesArrayCopy);

        // Return 0 if successful
        return 0;
        
    }
}