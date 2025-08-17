namespace AzzyShell.Commands;

class Set : Command
{
    public override int Execute(string[] args)
    {
       // Argument length validation
        if (!IsArgumentLengthValid(args, 3))
            return (int)ErrorCode.InvalidArguments;

        string name = args[1];
        string value = args[2];
        string type;

        // Determine type based on value
        if (int.TryParse(value, out _))
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
}
