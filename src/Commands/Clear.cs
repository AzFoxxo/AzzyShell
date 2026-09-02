namespace AzzyShell.Commands;

class Clear : Command
{
    public override int Execute(string[] args, CommandContext context)
    {
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

        try
        {
            if (Context.IsInteractiveOutput)
                Context.Output.Write("\u001b[2J\u001b[H");
        }
        catch (Exception ex)
        {
            PrintLine($"Failed to clear console: {ex.Message}", Colours.Red);
            return (int)ErrorCode.ConsoleClearFailed;
        }

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        "Clear the console.";
}