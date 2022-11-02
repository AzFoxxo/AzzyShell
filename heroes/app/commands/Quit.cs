namespace App.Commands;

using Heroes;

public class Quit : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Quit the app
        Application.Quit();

        // Return success
        return 0;
    }
}