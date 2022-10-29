namespace GameCode.Commands;

using Heroes;

public class Log : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 2) != 0) return 2;

        // Log the message
        Print(args[1]);

        // Return success
        return 0;
    }
}