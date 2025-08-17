namespace AzzyShell.Commands;

using Heroes;

public class Hacker : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

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
        return (int)ErrorCode.Success;
    }
}