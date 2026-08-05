namespace AzzyShell.Commands;

class Alias : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        switch (args.Length)
        {
            case 1:
                ListAliases();
                break;

            case 2:
                ShowAlias(args[1]);
                break;

            case 3:
                SetAlias(args[1], args[2]);
                break;

            default:
                return (int)ErrorCode.InvalidArguments;
        }

        return (int)ErrorCode.Success;
    }

    // display all the aliases
    private int ListAliases()
    {
        foreach (var alias in Shell.Aliases)
        {
            PrintLine($"alias {alias.Key} \"{alias.Value}\"", Colours.Cyan);
        }

        return (int)ErrorCode.Success;
    }

    // Show expanded alias for given alias
    private int ShowAlias(string name)
    {
        if (Shell.Aliases.ContainsKey(name))
        {
            PrintLine($"{name} \"{Shell.Aliases[name]}\"", Colours.Cyan);
            return (int)ErrorCode.Success;
        }

        PrintLine($"Error: Alias not found", Colours.Red);
        return (int)ErrorCode.AliasNotFound;
    }

    // Set an alias
    private int SetAlias(string name, string command)
    {
        // Create/update alias
        Shell.Aliases[name] = command;
        return (int)ErrorCode.Success;
    }

    public override string HelpString() => "Show all aliases, show an expanded alias or create an alias:\n`alias` Show all aliases\n`alias <alias_name>` show that alias\n`alias <alias_name> <aliased_command>`";
}
