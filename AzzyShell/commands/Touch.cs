namespace App.Commands;

using Heroes;

public class Touch : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 2) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Create the file
        File.Create(args[1]);

        // Return success
        return 0;
    }
}