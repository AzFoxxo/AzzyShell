namespace App.Commands;

using Heroes;

public abstract class Command : Hero
{
    public virtual int Execute(string[] args) => throw new NotImplementedException();

    public static int CheckArgLength(string[] args, int length, bool allowGreaterThanLength = false)
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

    /// <summary>
    /// Resolve variables
    /// </summary>
    /// <param name="args">Unresolved string array for command plus operands</param>
    /// <returns>Resolved string array</returns>
    /// <exception cref="ArgumentException">Thrown if an unrecognised opening bracket is encountered, or if a variable is not found, or if a closing bracket is missing.</exception>
    public static string[] VariableResolution(string[] args)
    {
        var shell = AzzyShell.GetInstance();
        var variables = shell.variables;

        // Build LUT table
        var variableDict = variables.ToDictionary(v => v.name);

        // Define opening to closing character map and what they mean
        var modeMap = new Dictionary<char, Func<Variables, string>>
    {
        { '{', v => v.value },
        { '[', v => v.type },
        { '(', v => v.name },
        { '<', v => variables.FindIndex(x => x.name == v.name).ToString() }
    };

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            foreach (var mode in modeMap.Keys)
            {
                char open = mode;
                char close = GetClosingChar(open);

                int startIndex = 0;

                while (startIndex < arg.Length)
                {
                    int openIndex = arg.IndexOf(open, startIndex);
                    if (openIndex == -1) break;

                    int closeIndex = arg.IndexOf(close, openIndex + 1);
                    if (closeIndex == -1)
                        throw new ArgumentException($"Unclosed variable placeholder: expected '{close}' after '{open}' in \"{arg}\".");

                    string varName = arg.Substring(openIndex + 1, closeIndex - openIndex - 1);

                    if (variableDict.TryGetValue(varName, out var variable))
                    {
                        string replacement = modeMap[open](variable);
                        arg = arg.Substring(0, openIndex) + replacement + arg.Substring(closeIndex + 1);
                        startIndex = openIndex + replacement.Length;
                    }
                    else
                    {
                        throw new ArgumentException($"Variable '{varName}' not found.");
                    }
                }
            }

            args[i] = arg;
        }

        return args;

        static char GetClosingChar(char open)
        {
            return open switch
            {
                '{' => '}',
                '[' => ']',
                '(' => ')',
                '<' => '>',
                _ => throw new ArgumentException($"Unknown opening character: {open}")
            };
        }
    }
}