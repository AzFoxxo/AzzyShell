namespace App.Commands;

using Heroes;

public class FizzBuzz : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 2) != 0) return 2;

        // Try to parse the number
        if (!int.TryParse(args[1], out int number))
        {
            // Print error
            Print("Invalid number");

            // Return error
            return 1;
        }

        for (int i = 0; i < number; i++) {
            var _string = "";
            if (i % 3 == 0) _string += "Fizz";
            if (i % 5 == 0) _string += "Buzz";
            if (_string == "") _string = i.ToString();
            Print(_string);
        }

        // Return success
        return 0;
    }
}