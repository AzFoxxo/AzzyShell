namespace AzzyShell.Commands;

class Quit : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

        // Quit the app
        Program.Running = false;

        // Return success
        return (int)ErrorCode.Success;
    }

    public override string HelpString()
    {
        return "quits the shell.";
    }
}