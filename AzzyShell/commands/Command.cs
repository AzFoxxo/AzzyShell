namespace App.Commands;

using Heroes;

public class Command : Hero
{
    public virtual int Execute(string[] args) => throw new NotImplementedException();

    public virtual void PrintHelp(string[] args) => PrintLine($"No help available for this command - {args[0]}", Colours.Red);

    public int CheckArgLength(string[] args, int length, bool allowGreaterThanLength = false)
    {
        if (args.Length != length)
        {
            // Allow commands to have more arguments than the minimum
            if (allowGreaterThanLength && args.Length > length) return 0;

            // Print error
            PrintLine($"'{args[0]}' takes {length - 1} argument(s), not {args.Length - 1}", Colours.Red);
            return 2; // Invalid arguments
        }
        else
        {
            return 0; // Success
        }
    }

    public string[] VariableTranslation(string[] args)
    {
        // Loop through each argument
        var varList = AzzyShell.GetInstance().variables;
        for (int i = 0; i < args.Length; i++)
        {
            // Loop the current arg
            for (int j = 0; j < args[i].Length; j++)
            {
                // Variable mode (type, name, value, index)
                char opening;
                char closing;
                if (args[i][j] == '{') { opening = '{'; closing = '}'; }
                else if (args[i][j] == '[') { opening = '['; closing = ']'; }
                else if (args[i][j] == '(') { opening = '('; closing = ')'; }
                else if (args[i][j] == '<') { opening = '<'; closing = '>'; }
                else { continue; }

                // Check if the current arg contains a variable
                string variable = "";
                for (int k = j + 1; k < args[i].Length; k++)
                {
                    if (args[i][k] == closing)
                    {
                        break;
                    }
                    else
                    {
                        variable += args[i][k];
                    }
                }
                // Get the variable value
                string variableInfo = "";
                foreach (Variables var in varList)
                {
                    if (var.name == variable)
                    {
                        // Check if opening character is `{` (value)
                        if (opening == '{') variableInfo = var.value;
                        // Check if opening character is `[` (type)
                        else if (opening == '[') variableInfo = var.type;
                        // Check if opening character is `(` (name)
                        else if (opening == '(') variableInfo = var.name;
                        // Check if opening character is `<` (index)
                        // Get the index of the variable in the variable list
                        else if (opening == '<')
                        {
                            for (int k = 0; k < varList.Count; k++)
                            {
                                if (varList[k].name == variable)
                                {
                                    variableInfo = k.ToString();
                                    break;
                                }
                            }
                        }

                        // Variable found, break the loop
                        break;
                    }
                }
                // Replace the variable with it's value, type, name or index
                args[i] = args[i].Replace($"{opening}{variable}{closing}", variableInfo);
            }
        }

        // Return the translated argument
        return args;
    }
}