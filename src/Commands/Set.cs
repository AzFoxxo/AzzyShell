namespace AzzyShell.Commands;

class Set : Command
{
    public override int Execute(string[] args, CommandContext context)
    {
        if (args.Length is < 2 or > 3)
        {
            PrintLine($"'{args[0]}' takes 1 or 2 argument(s), {args.Length - 1} argument(s) given.", Colours.Red);
            return (int)ErrorCode.InvalidArguments;
        }

        string name = args[1];
        if (args.Length == 2)
        {
            Shell.SetVariable(name, string.Empty, VariableType.Null.ToString());
            return (int)ErrorCode.Success;
        }

        string value = ScriptText.UnwrapLiteral(args[2]);

        string type = DetectType(value).ToString();

        Shell.SetVariable(name, value, type);

        return (int)ErrorCode.Success;
    }

    private static VariableType DetectType(string value)
    {
        if (int.TryParse(value, out _))
            return VariableType.Int;

        if (double.TryParse(value, out _))
            return VariableType.Double;

        if (bool.TryParse(value, out _))
            return VariableType.Bool;

        return VariableType.String;
    }

    public override string HelpString() =>
    """
    Define or update a variable:

    `set <name> [value]`

    Automatically detects types:
        Int
        Double
        Bool
        String
        Null

    Examples:
        set age 20
        set enabled true
        set name Az
        set empty
    """;
}