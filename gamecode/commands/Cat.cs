namespace GameCode.Commands;

using Heroes;

public class Cat : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 2) != 0) return 2;

        // Check if the file exists
        if (!File.Exists(args[1]))
        {
            Print("File not found");
            return 1;
        }

        // Read the file
        string[] lines = File.ReadAllLines(args[1]);

        // Print the file
        foreach (string line in lines)
        {
            Print(line);
        }

        // Return success
        return 0;
    }
}