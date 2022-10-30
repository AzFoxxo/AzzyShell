namespace App.Commands;

using Heroes;

public class Command : Hero {
    public virtual int Execute(string[] args) => throw new NotImplementedException();

    public virtual void PrintHelp(string[] args) => Print($"No help available for this command - {args[0]}");

     public int CheckArgLength(string[] args, int length) {
        if (args.Length != length) {
            Print($"'{args[0]}' takes {length-1} argument(s), not {args.Length-1}");
            return 2; // Invalid arguments
        } else {
            return 0; // Success
        }
    }
}