namespace AzzyShell.Commands;

class Unalias : Command
{
    public override int Execute(string[] args)
    {
        if (!IsArgumentLengthValid(args, 2))
            return (int)ErrorCode.InvalidArguments;

        return RemoveAlias(args[1]);
    }

    // Remove an alias
    private int RemoveAlias(string name)
    {
        if (!Shell.Aliases.ContainsKey(name))
        {
            PrintLine($"Error: Alias '{name}' not found.", Colours.Red);
            return (int)ErrorCode.AliasNotFound;
        }

        Shell.Aliases.Remove(name);

        PrintLine($"Removed alias '{name}'.", Colours.Cyan);
        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        """
        Remove a command alias:

        `unalias <alias_name>`
            Remove the specified alias.

        Examples:
            unalias ll
            unalias gs
        """;
}