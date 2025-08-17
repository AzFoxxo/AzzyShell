namespace App.Commands;

using Heroes;

public class Remove : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 2) != 0) return 2;

        // Flags
        bool file = false;
        bool dir = false;

        // Check if the file exists
        if (File.Exists(args[1]))
        {
            // Set the file flag
            file = true;
        }

        // Check if the directory exists
        if (Directory.Exists(args[1]))
        {
            // Set the file flag
            dir = true;
        }

        // Check if the file flag is set
        if (file)
        {
            // Delete the file
            File.Delete(args[1]);
        } else if (dir)
        {
            // Delete the directory recursively
            Directory.Delete(args[1], true);
        } else
        {
            // Return error
            return 3;
        }

        // Return success
        return 0;
    }
}