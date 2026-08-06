namespace AzzyShell.Commands;

class Hacker : Command
{
    public override int Execute(string[] args)
    {
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

        while (true)
        {
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                break;

            int random = Random.Shared.Next(0, 2);

            Print(random.ToString(), Colours.Green);
        }

        PrintLine("");

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        "Display a Matrix-style stream of 0s and 1s (press Escape to quit).";
}