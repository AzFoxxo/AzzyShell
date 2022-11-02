namespace App.Commands;

using Heroes;

public class LS : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // List all the directories in the current directory
        Print("Directories:");
        foreach (string dir in Directory.GetDirectories(Directory.GetCurrentDirectory()))
        {
            Print(dir);
        }
        // List all the directories in the current directory
        Print("Files:");
        foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory()))
        {
            Print(file);
        }


        // Return success
        return 0;
    }
}