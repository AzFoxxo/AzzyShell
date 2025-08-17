namespace AzzyShell.Commands;

using Heroes;

public class GayText : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 2, allowGreaterThanLength: true))
        {
            return (int)ErrorCode.InvalidArguments;
        }

        // Join message
        string message = string.Join(" ", args, 1, args.Length - 1);

        // Print the message in gay colours
        GayPrint(message);

        // Return success
        return (int)ErrorCode.Success;
    }
}
