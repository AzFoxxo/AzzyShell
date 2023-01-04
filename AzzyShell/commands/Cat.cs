namespace App.Commands;

using Heroes;

public class Cat : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 2, allowGreaterThanLength: true) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Counter
        int i = 1;

        // Check if the file exists
        foreach (var file in args.Skip(1))
        {
            // Increment counter
            i++;

            // Check if the file exists
            if (!File.Exists(file))
            {
                PrintLine($"File not found {i}", Colours.Red);
                return 1;
            }

            // Read the file
            string[] lines = File.ReadAllLines(file);

            // Print the file
            PrintLine($"{file}:", Colours.Blue);
            foreach (string line in lines)
            {
                PrintLine(line, Colours.Yellow);
            }
        }

        // Return success
        return 0;
    }

    public override string Description => "Print the contents of a file";

    public override void PrintHelp(string[] args)
    {
        PrintLine(Description, Colours.Green);
        PrintLine($"Usage: {this.GetType().Name} <file>", Colours.Green);
    }
}