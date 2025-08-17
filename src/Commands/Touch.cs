namespace AzzyShell.Commands;

class Touch : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 2))
            return (int)ErrorCode.InvalidArguments;

        // Create the file
        File.Create(args[1]);

        // Return success
        return (int)ErrorCode.Success;
    }
}