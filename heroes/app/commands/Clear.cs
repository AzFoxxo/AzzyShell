namespace App.Commands;

using Heroes;

public class Clear : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // List all the directories in the current directory
        Console.Clear();


        // Return success
        return 0;
    }
}