namespace AzzyShell.Commands;

class Log : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 2, allowGreaterThanLength: true))
        {
            return (int)ErrorCode.InvalidArguments;
        }

        // Join message
        string logMessage = string.Join(" ", args, 1, args.Length - 1);

        // Log the message
        PrintLine(logMessage);

        // Return success
        return (int)ErrorCode.Success;
    }

    public override string HelpString() => "Log a message to the console: <arg1> <arg2> etc.";
}
