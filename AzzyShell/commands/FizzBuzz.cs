namespace App.Commands;

using Heroes;

public class FizzBuzz : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 2) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Try to parse the number
        if (!int.TryParse(args[1], out int number))
        {
            // Print error
            PrintLine("Invalid number");

            // Return error
            return 1;
        }

        for (int i = 0; i <= number; i++) {
            var output = "";
            if (i % 3 == 0) output += "Fizz";
            if (i % 5 == 0) output += "Buzz";
            if (output == "") output = i.ToString();
            PrintLine(output);
        }

        // Return success
        return 0;
    }

    public override string Description => "Print the FizzBuzz sequence";

    public override void PrintHelp(string[] args)
    {
        PrintLine(Description, Colours.Green);
        PrintLine($"Usage: {this.GetType().Name} [args]", Colours.Green);
        PrintLine("Args:", Colours.Green);
        PrintLine("  arg1 - Number to count to*", Colours.Green);
    }
}