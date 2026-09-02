namespace AzzyShell.Commands;

class History : Command
{
    public override int Execute(string[] args, CommandContext context)
    {
        if (!IsArgumentLengthValid(args, 1, allowGreaterThanLength: true))
            return (int)ErrorCode.InvalidArguments;

        if (args.Length == 2)
        {
            if (!args[1].Equals("clear", StringComparison.OrdinalIgnoreCase))
                return (int)ErrorCode.InvalidArguments;

            File.WriteAllText(Shell.historyFile, string.Empty);

            PrintLine("History cleared.");

            return (int)ErrorCode.Success;
        }

        if (!File.Exists(Shell.historyFile))
        {
            using (File.Create(Shell.historyFile)) { }
        }

        string[] history = File.ReadAllLines(Shell.historyFile);

        for (int i = history.Length - 1; i >= 0; i--)
        {
            PrintLine($"{history.Length - i}: {history[i]}");
        }

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        """
        Display command history:

        `history`
            Show command history.

        `history clear`
            Clear command history.
        """;
}