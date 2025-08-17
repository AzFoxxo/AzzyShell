namespace AzzyShell.Commands;

using Heroes;

public abstract class Command : Hero
{
    protected Azzy Shell { get; private set; } = Azzy.GetInstance();

    public virtual int Execute(string[] args) => throw new NotImplementedException();

    public static bool IsArgumentLengthValid(string[] args, int length, bool allowGreaterThanLength = false)
    {
        // Check if argument length is what is requested
        if (args.Length != length)
        {
            // Allow commands to have more arguments but not fewer
            if (allowGreaterThanLength && args.Length > length) return true;

            // Display error message
            PrintLine($"'{args[0]}' takes {length - 1} argument(s), {args.Length - 1} argument(s) given.", Colours.Red);
            return false;  // Return false when the length is invalid
        }

        return true;  // Return true when the length matches
    }


    /// <summary>
    /// Resolve variables
    /// </summary>
    /// <param name="args">Unresolved string array for command plus operands</param>
    /// <returns>Resolved string array</returns>
    /// <exception cref="ArgumentException">Thrown if an unrecognised opening bracket is encountered, or if a variable is not found, or if a closing bracket is missing.</exception>
    public static string[] VariableResolution(string[] args)
    {
        var variables = Azzy.GetInstance().Variables;

        // Define opening to closing character map and what they extract
        var modeMap = new Dictionary<char, Func<ShellVariable, string>>
    {
        { '{', v => v.Value },
        { '[', v => v.Type },
        { '(', v => v.Name },
        { '<', v => variables.Keys.ToList().IndexOf(v.Name).ToString() }
    };

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            foreach (var open in modeMap.Keys)
            {
                var close = GetClosingChar(open);
                int startIndex = 0;

                while (startIndex < arg.Length)
                {
                    int openIndex = arg.IndexOf(open, startIndex);
                    if (openIndex == -1) break;

                    int closeIndex = arg.IndexOf(close, openIndex + 1);
                    if (closeIndex == -1)
                        throw new ArgumentException($"Unclosed variable placeholder: expected '{close}' after '{open}' in \"{arg}\".");

                    // Span-based efficient slicing
                    var span = arg.AsSpan();
                    var varNameSpan = span.Slice(openIndex + 1, closeIndex - openIndex - 1);
                    var varName = varNameSpan.ToString();

                    if (!variables.TryGetValue(varName, out var variable))
                        throw new ArgumentException($"Variable '{varName}' not found.");

                    string replacement = modeMap[open](variable);

                    // Efficient string replacement using spans
                    arg = string.Concat(
                        span[..openIndex],
                        replacement,
                        span[(closeIndex + 1)..]
                    );

                    startIndex = openIndex + replacement.Length;
                }
            }

            args[i] = arg;
        }

        return args;

        static char GetClosingChar(char open) => open switch
        {
            '{' => '}',
            '[' => ']',
            '(' => ')',
            '<' => '>',
            _ => throw new ArgumentException($"Unknown opening character: {open}")
        };
    }
}