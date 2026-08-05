namespace AzzyShell.Commands;

class MKDir : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 2))
            return (int)ErrorCode.InvalidArguments;

        // Make the directory
        Directory.CreateDirectory(args[1]);

        // Return success
        return (int)ErrorCode.Success;
    }

    public override string HelpString() => "Creates a directory: <dir>";
}