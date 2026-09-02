namespace AzzyShell.Commands;

class Quit : Command
{
    public override int Execute(string[] args, CommandContext context)
    {
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

        Program.Running = false;

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        "Exit AzzyShell.";
}