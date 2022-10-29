namespace GameCode.Commands;

using Heroes;

public class Quit : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1) != 0) return 2;

        // Quit the app
        App.End();

        // Return success
        return 0;
    }
}