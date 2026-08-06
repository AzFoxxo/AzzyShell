namespace AzzyShell.Commands;

class Vars : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

        // Iterate over variables dictionary
        int index = 0;
        foreach (var kvp in Shell.Variables)
        {
            var variable = kvp.Value;

            bool isString = variable.Type == "String";
            bool isNull = variable.Type == "Null";

            Print($"{index}: ", Colours.Yellow);
            Print($"{variable.Type} ", Colours.DarkRed);
            Print($"{variable.Name} ", Colours.Green);
            Print("= ", Colours.White);
            if (isString)
                PrintLine($"\"{ScriptText.UnwrapLiteral(variable.Value)}\"", Colours.Blue);
            else if (isNull)
                PrintLine("null", Colours.Blue);
            else
                PrintLine($"{variable.Value}", Colours.Blue);

            index++;
        }

        // Return success
        return (int)ErrorCode.Success;
    }

    public override string HelpString() => "Displays the variables and their types.";
}
