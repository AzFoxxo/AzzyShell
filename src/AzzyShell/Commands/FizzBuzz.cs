namespace AzzyShell.Commands;

public class FizzBuzz : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

        // Try to parse the number
        if (!int.TryParse(args[1], out int number))
        {
            // Print error
            PrintLine("Invalid number");

            // Return error
            return (int)ErrorCode.InvalidArguments;
        }

        for (int i = 0; i <= number; i++) {
            var output = "";
            if (i % 3 == 0) output += "Fizz";
            if (i % 5 == 0) output += "Buzz";
            if (output == "") output = i.ToString();
            PrintLine(output);
        }

        // Return success
        return (int)ErrorCode.Success;
    }
}