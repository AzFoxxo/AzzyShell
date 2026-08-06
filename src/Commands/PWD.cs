namespace AzzyShell.Commands;

class Pwd : Command
{
    public override int Execute(string[] args)
    {
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

        PrintLine(Directory.GetCurrentDirectory(), Colours.Blue);

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        "Print the current working directory.";
}