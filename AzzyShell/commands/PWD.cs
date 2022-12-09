namespace App.Commands;

using Heroes;

public class PWD : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Get the current directory checking for errors
        var dir = Directory.GetCurrentDirectory();
        if (dir == null)
        {
            PrintLine("Error getting current directory");
            return 1;
        }

        // Print the current directory
        PrintLine(dir, Colours.Blue);


        // Return success
        return 0;
    }
}