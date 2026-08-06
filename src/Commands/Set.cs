namespace AzzyShell.Commands;

class Set : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (args.Length < 2)
        {
            PrintLine("'set' takes at least 1 argument(s), 0 argument(s) given.", Colours.Red);
            return (int)ErrorCode.InvalidArguments;
        }

        string name = args[1];
        string value = args.Length >= 3 ? string.Join(' ', args.Skip(2)) : string.Empty;
        string type;

        if (args.Length < 3)
        {
            type = "Null";
        }
        else if (int.TryParse(value, out _))
        {
            type = "Int";
        }
        else if (double.TryParse(value, out _))
        {
            type = "Double";
        }
        else if (bool.TryParse(value, out _))
        {
            type = "Bool";
        }
        else
        {
            type = "String";
        }

        // Use the SetVariable method to add or update the variable
        Shell.SetVariable(name, value, type);

        return (int)ErrorCode.Success;
    }

    public override string HelpString() => "Define/set a variable: <var_name> [value] (supports string, bool, double, int, null)";
}
