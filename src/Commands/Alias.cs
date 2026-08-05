namespace AzzyShell.Commands;

class Alias : Command
{
    public override int Execute(string[] args)
    {
        // Show all aliases
        if (args.Length == 1)
        {
            return ListAliases();
        }

        // Show a specific alias
        if (args.Length == 2)
        {
            return ShowAlias(args[1]);
        }

        // Create/update alias
        if (args.Length >= 3)
        {
            return SetAlias(args[1], string.Join(' ', args.Skip(2)));
        }

        return (int)ErrorCode.InvalidArguments;
    }

    // Display all aliases
    private int ListAliases()
    {
        if (Shell.Aliases.Count == 0)
        {
            PrintLine("No aliases defined.", Colours.Yellow);
            return (int)ErrorCode.Success;
        }

        foreach (var alias in Shell.Aliases)
        {
            PrintLine($"alias {alias.Key} \"{alias.Value}\"", Colours.Cyan);
        }

        return (int)ErrorCode.Success;
    }

    // Show expanded alias for given alias
    private int ShowAlias(string name)
    {
        if (Shell.Aliases.TryGetValue(name, out var command))
        {
            PrintLine($"alias {name} \"{command}\"", Colours.Cyan);
            return (int)ErrorCode.Success;
        }

        PrintLine($"Error: Alias '{name}' not found.", Colours.Red);
        return (int)ErrorCode.AliasNotFound;
    }

    // Set an alias
    private int SetAlias(string name, string command)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(command))
        {
            PrintLine("Alias name and command cannot be empty.", Colours.Red);
            return (int)ErrorCode.InvalidArguments;
        }

        Shell.Aliases[name] = command;

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        """
        Manage command aliases:

        `alias`
            Show all aliases.

        `alias <alias_name>`
            Show a specific alias.

        `alias <alias_name> <command>`
            Create or update an alias.

        Examples:
            alias ll ls -lah
            alias home cd ~
            alias gs git status
        """;
}