namespace AzzyShell.Commands;

class Vars : Command
{
    public override int Execute(string[] args, CommandContext context)
    {
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

        int index = 0;

        foreach (var variable in Shell.Variables.Values)
        {
            Print($"{index}: ", Colours.Yellow);
            Print($"{variable.Type} ", Colours.DarkRed);
            Print($"{variable.Name} ", Colours.Green);
            Print("= ", Colours.White);

            switch (variable.Type)
            {
                case "String":
                    PrintLine($"\"{ScriptText.UnwrapLiteral(variable.Value)}\"", Colours.Blue);
                    break;

                case "Null":
                    PrintLine("null", Colours.Blue);
                    break;

                default:
                    PrintLine(variable.Value, Colours.Blue);
                    break;
            }

            index++;
        }

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        """
        Display all defined variables and their types.

        Shows:
            - Variable index
            - Variable type
            - Variable name
            - Variable value

        Supported types:
            Int
            Double
            Bool
            String
            Null
        """;
}