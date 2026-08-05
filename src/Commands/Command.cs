namespace AzzyShell.Commands;

abstract class Command : HeroesPatch
{
    protected Azzy Shell { get; private set; } = Azzy.GetInstance();

    public virtual int Execute(string[] args) => throw new NotImplementedException();

    public static bool IsArgumentLengthValid(string[] args, int length, bool allowGreaterThanLength = false)
    {
        // Check if argument length is what is requested
        if (args.Length != length)
        {
            // Allow commands to have more arguments but not fewer
            if (allowGreaterThanLength && args.Length > length) return true;

            // Display error message
            PrintLine($"'{args[0]}' takes {length - 1} argument(s), {args.Length - 1} argument(s) given.", Colours.Red);
            return false;  // Return false when the length is invalid
        }

        return true;  // Return true when the length matches
    }

    public virtual string HelpString()
    {
        return "Command does not contain a help page.";
    }
}