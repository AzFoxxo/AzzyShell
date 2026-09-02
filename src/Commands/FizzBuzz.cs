namespace AzzyShell.Commands;

class FizzBuzz : Command
{
    public override int Execute(string[] args, CommandContext context)
    {
        if (!IsArgumentLengthValid(args, 2))
            return (int)ErrorCode.InvalidArguments;

        if (!int.TryParse(args[1], out int limit))
        {
            PrintLine("Invalid number.", Colours.Red);
            return (int)ErrorCode.InvalidArguments;
        }

        for (int i = 1; i <= limit; i++)
        {
            string output = string.Empty;

            if (i % 3 == 0)
                output += "Fizz";

            if (i % 5 == 0)
                output += "Buzz";

            PrintLine(string.IsNullOrEmpty(output) ? i.ToString() : output);
        }

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        "Print the FizzBuzz sequence up to a number: <limit>";
}