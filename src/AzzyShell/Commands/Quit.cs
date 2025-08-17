namespace AzzyShell.Commands;

using Heroes;

public class Quit : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

        // Quit the app
        Application.Quit();

        // Return success
        return (int)ErrorCode.Success;
    }
}