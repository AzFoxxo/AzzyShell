namespace AzzyShell.Commands;

abstract class Command
{
    protected Azzy Shell { get; private set; } = Azzy.GetInstance();
    protected CommandContext Context { get; private set; } = CommandContext.Interactive;

    internal void SetContext(CommandContext context) => Context = context;

    public virtual int Execute(string[] args, CommandContext context) => throw new NotImplementedException();

    protected void Print(string text, Colours colour = Colours.White)
    {
        WriteWithColour(Context.Output, text, colour, newline: false);
    }

    protected void PrintLine(object text) => Context.Output.WriteLine(text);

    protected void PrintLine(string text, Colours colour)
    {
        TextWriter writer = colour == Colours.Red ? Context.Error : Context.Output;
        WriteWithColour(writer, text, colour, newline: true);
    }

    protected void GayPrint(string message, bool newline = true)
    {
        Print(message);
        if (newline)
            PrintLine("");
    }

    private static void WriteWithColour(TextWriter writer, string text, Colours colour, bool newline)
    {
        bool isInteractiveOutput = !Console.IsOutputRedirected && ReferenceEquals(writer, Console.Out);
        bool isInteractiveError = !Console.IsErrorRedirected && ReferenceEquals(writer, Console.Error);

        if (!isInteractiveOutput && !isInteractiveError)
        {
            if (newline)
                writer.WriteLine(text);
            else
                writer.Write(text);

            return;
        }

        writer.Write($"\u001b[{Colour.ToAnsiCode(colour)}m");
        if (newline)
            writer.WriteLine(text);
        else
            writer.Write(text);
        writer.Write("\u001b[0m");
    }

    public bool IsArgumentLengthValid(string[] args, int length, bool allowGreaterThanLength = false)
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

internal sealed record CommandContext(TextReader Input, TextWriter Output, TextWriter Error)
{
    public static CommandContext Interactive => new(Console.In, Console.Out, Console.Error);

    public bool IsInteractiveOutput => ReferenceEquals(Output, Console.Out) && !Console.IsOutputRedirected;
}

public enum Colours
{
    Black,
    Blue,
    Cyan,
    DarkBlue,
    DarkCyan,
    DarkGray,
    DarkGreen,
    DarkMagenta,
    DarkRed,
    DarkYellow,
    Gray,
    Green,
    Magenta,
    Red,
    White,
    Yellow
}

public static class Colour
{
    public static int ToAnsiCode(Colours colour) =>
        colour switch
        {
            Colours.Black => 30,
            Colours.Red => 31,
            Colours.Green => 32,
            Colours.Yellow => 33,
            Colours.Blue => 34,
            Colours.Magenta => 35,
            Colours.Cyan => 36,
            Colours.Gray => 90,
            Colours.DarkBlue => 34,
            Colours.DarkCyan => 36,
            Colours.DarkGray => 90,
            Colours.DarkGreen => 32,
            Colours.DarkMagenta => 35,
            Colours.DarkRed => 31,
            Colours.DarkYellow => 33,
            Colours.White => 37,
            _ => 37,
        };
}