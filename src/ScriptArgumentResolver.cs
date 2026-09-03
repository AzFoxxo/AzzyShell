namespace AzzyShell;

internal static class ScriptArgumentResolver
{
    public static string[] Resolve(Azzy shell, IReadOnlyList<string> args)
    {
        var resolved = new string[args.Count];

        for (int i = 0; i < args.Count; i++)
        {
            resolved[i] = ResolveToken(shell, args[i]);
        }

        return resolved;
    }

    private static string ResolveToken(Azzy shell, string token)
    {
        bool isLiteral = ScriptText.IsLiteral(token);
        string value = isLiteral ? ScriptText.UnwrapLiteral(token) : token;

        if (!isLiteral)
        {
            value = ResolveVariables(shell, value);
            value = ResolvePath(value);
        }

        return isLiteral ? ScriptText.MarkLiteral(value) : value;
    }

    private static string ResolveVariables(Azzy shell, string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var builder = new System.Text.StringBuilder();

        for (int index = 0; index < value.Length; index++)
        {
            char current = value[index];

            if (!IsOpenToken(current))
            {
                builder.Append(current);
                continue;
            }

            char close = GetClosingToken(current);
            int closeIndex = value.IndexOf(close, index + 1);

            if (closeIndex == -1)
                throw new ArgumentException($"Unclosed variable placeholder: expected '{close}' after '{current}' in \"{value}\".");

            string name = value[(index + 1)..closeIndex];
            builder.Append(ResolveVariable(shell, current, name));
            index = closeIndex;
        }

        return builder.ToString();
    }

    private static string ResolvePath(string value)
    {
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (value == "~")
            return home;

        if (value.StartsWith("~/", StringComparison.Ordinal))
            return Path.Combine(home, value[2..]);

        return value;
    }

    private static string ResolveVariable(Azzy shell, char open, string name)
    {
        if (!shell.Variables.TryGetValue(name, out var variable))
            throw new ArgumentException($"Variable '{name}' not found.");

        return open switch
        {
            '{' => ScriptText.UnwrapLiteral(variable.Value),
            '[' => variable.Type,
            '(' => variable.Name,
            '<' => shell.Variables.Keys.ToList().IndexOf(variable.Name).ToString(),
            _ => throw new ArgumentException($"Unknown opening character: {open}")
        };
    }

    private static bool IsOpenToken(char value) => value is '{' or '[' or '(' or '<';

    private static char GetClosingToken(char value) => value switch
    {
        '{' => '}',
        '[' => ']',
        '(' => ')',
        '<' => '>',
        _ => throw new ArgumentException($"Unknown opening character: {value}")
    };
}