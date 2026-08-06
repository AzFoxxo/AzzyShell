namespace AzzyShell.Commands;

class Log : Command
{
    public override int Execute(string[] args)
    {
        if (!IsArgumentLengthValid(args, 2, allowGreaterThanLength: true))
            return (int)ErrorCode.InvalidArguments;

        string message = string.Join(' ', args.Skip(1));

        PrintLine(message);

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        "Print a message to the console: <message>";
}