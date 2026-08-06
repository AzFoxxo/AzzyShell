namespace AzzyShell.Commands;

class Set : Command
{
    public override int Execute(string[] args)
    {
        if (!IsArgumentLengthValid(args, 3))
            return (int)ErrorCode.InvalidArguments;

        string name = args[1];
        string value = args[2];

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