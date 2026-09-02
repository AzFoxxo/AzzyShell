namespace AzzyShell.Commands;

class Sort : Command
{
    public override int Execute(string[] args, CommandContext context)
    {
        if (!IsArgumentLengthValid(args, 1, allowGreaterThanLength: true))
            return (int)ErrorCode.InvalidArguments;

        bool descending = args.Length > 1 && args[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

        // Read from stdin
        string input = Context.Input.ReadToEnd();

        if (string.IsNullOrWhiteSpace(input))
            return (int)ErrorCode.Success;

        // Decide whether to sort words or lines.
        bool sortLines = input.Contains('\n');

        if (sortLines)
        {
            var lines = input
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.TrimEnd('\r'));

            IEnumerable<string> sorted = descending
                ? lines.OrderByDescending(l => l, StringComparer.OrdinalIgnoreCase)
                : lines.OrderBy(l => l, StringComparer.OrdinalIgnoreCase);

            foreach (var line in sorted)
                PrintLine(line);
        }
        else
        {
            var words = input
                .Split((char[])null!, StringSplitOptions.RemoveEmptyEntries);

            IEnumerable<string> sorted = descending
                ? words.OrderByDescending(w => w, StringComparer.OrdinalIgnoreCase)
                : words.OrderBy(w => w, StringComparer.OrdinalIgnoreCase);

            PrintLine(string.Join(' ', sorted));
        }

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        """
        Sort text from standard input.

        `sort`
            Sort alphabetically (A-Z).

        `sort desc`
            Sort in reverse alphabetical order (Z-A).

        Examples:
            ls | sort
            log "pear apple orange" | sort
            cat names.txt | sort desc
        """;
}