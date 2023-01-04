namespace App.Commands;

using Heroes;

public class Hacker : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Infinite loop
        while (true)
        {
            // Randomly choose a number between 0 and 1 (dotnet)
            int random = new Random().Next(0, 2);

            // Print the logo
            Print(random.ToString(), Colours.Green);

            // Break loop if escape key is pressed (dotnet)
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) break;
        }

        // Print a new line
        PrintLine("");

        // Return success
        return 0;
    }

    public override string Description => "Hacker mode";

    public override void PrintHelp(string[] args)
    {
        PrintLine(Description, Colours.Green);
        PrintLine($"Usage: {this.GetType().Name}", Colours.Green);
    }
}