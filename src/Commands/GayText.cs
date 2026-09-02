namespace AzzyShell.Commands;

class GayText : Command
{
    public override int Execute(string[] args, CommandContext context)
    {
        if (!IsArgumentLengthValid(args, 2, allowGreaterThanLength: true))
            return (int)ErrorCode.InvalidArguments;

        string message = string.Join(' ', args.Skip(1));

        GayPrint(message);

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        "Print a message to the console in rainbow colours: <message>";
}