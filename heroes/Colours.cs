namespace Heroes;

// Enum of supported supported colours
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
    // Convert the colour to a ConsoleColor
    public static ConsoleColor Convert(Colours colour)
    {
        // Return the colour
        return colour switch
        {
            Colours.Black => ConsoleColor.Black,
            Colours.Blue => ConsoleColor.Blue,
            Colours.Cyan => ConsoleColor.Cyan,
            Colours.DarkBlue => ConsoleColor.DarkBlue,
            Colours.DarkCyan => ConsoleColor.DarkCyan,
            Colours.DarkGray => ConsoleColor.DarkGray,
            Colours.DarkGreen => ConsoleColor.DarkGreen,
            Colours.DarkMagenta => ConsoleColor.DarkMagenta,
            Colours.DarkRed => ConsoleColor.DarkRed,
            Colours.DarkYellow => ConsoleColor.DarkYellow,
            Colours.Gray => ConsoleColor.Gray,
            Colours.Green => ConsoleColor.Green,
            Colours.Magenta => ConsoleColor.Magenta,
            Colours.Red => ConsoleColor.Red,
            Colours.White => ConsoleColor.White,
            Colours.Yellow => ConsoleColor.Yellow,
            _ => ConsoleColor.White,
        };
    }
}