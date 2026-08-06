/*
*   This file is a stripped down reimplemention
*   of the logic AzzyShell used from Heroes
*
*   The code should be a straight drop-in for Hero
*   for common commands (like IO and colours only)
*   -----
*   Implements
*   -----
*   Read
*   Print
*   PrintLine
*   ReadInline
*   GayPrint
*   ----
*   Colours enum
*   Colour class
*   
*/

namespace AzzyShell;

class HeroesPatch
{
    ///<summary>Print a message to the console on line in a colour.</summary>
    ///<param name="message">The message to print.</param>
    ///<param name="colour">The colour to print the message in.</param>
    protected static void Print(string text, Colours colour = Colours.White)
    {
        Console.ForegroundColor = Colour.Convert(colour);
        Console.Write(text);
        Console.ResetColor();
    }

    ///<summary>Read input from the user.</summary>
    ///<param name="prompt">The prompt to display.</param>
    ///<returns>The input from the user.</returns>
    protected static string Read(string prompt)
    {
        // Print the prompt
        Print(prompt);

        // Return the input (check if the input is null or empty)
        return Console.ReadLine() ?? "";
    }

    /// <summary>
    /// Read input from the user in a colour, with inline editing (arrow keys, insert, backspace).
    /// </summary>
    /// <param name="prompt">The prompt to display.</param>
    /// <param name="colour">The colour to display the prompt in. Optional.</param>
    /// <returns>The input from the user.</returns>
    protected static string ReadInline(string prompt, Colours colour = Colours.White)
    {
        // Print the prompt with colour
        Print(prompt, colour);

        var input = new System.Text.StringBuilder();
        int cursorPos = 0;
        int promptWidth = GetDisplayWidth(prompt);
        int historyIndex = -1;
        string draftInput = string.Empty;
        int renderedLength = 0;
        var shellHistory = Azzy.GetInstance().HistoryEntries;

        void ReplaceInput(string newValue)
        {
            input.Clear();
            input.Append(newValue);
            cursorPos = input.Length;
        }

        void RedrawInput()
        {
            int currentLine = Console.CursorTop;
            int cursorLeft = promptWidth;
            int clearWidth = Math.Max(renderedLength, input.Length) + 1;

            Console.SetCursorPosition(cursorLeft, currentLine);
            Console.Write(new string(' ', clearWidth));
            Console.SetCursorPosition(cursorLeft, currentLine);
            Console.Write(input.ToString());
            renderedLength = input.Length;
            Console.SetCursorPosition(cursorLeft + cursorPos, currentLine);
        }

        void ShowHistoryEntry(int newIndex)
        {
            if (newIndex < 0 || newIndex >= shellHistory.Count)
                return;

            if (historyIndex == -1)
                draftInput = input.ToString();

            historyIndex = newIndex;
            ReplaceInput(shellHistory[historyIndex]);
            RedrawInput();
        }

        void ExitHistoryMode()
        {
            if (historyIndex == -1)
                return;

            historyIndex = -1;
            ReplaceInput(draftInput);
            RedrawInput();
        }

        void MoveHistoryUp()
        {
            if (shellHistory.Count == 0)
                return;

            if (historyIndex == -1)
            {
                ShowHistoryEntry(shellHistory.Count - 1);
                return;
            }

            if (historyIndex > 0)
            {
                ShowHistoryEntry(historyIndex - 1);
            }
        }

        void MoveHistoryDown()
        {
            if (historyIndex == -1)
                return;

            if (historyIndex < shellHistory.Count - 1)
            {
                ShowHistoryEntry(historyIndex + 1);
                return;
            }

            ExitHistoryMode();
        }

        while (true)
        {
            var keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow)
            {
                if (cursorPos > 0)
                {
                    cursorPos--;
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                }
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow)
            {
                if (cursorPos < input.Length)
                {
                    cursorPos++;
                    Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                }
            }
            else if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                MoveHistoryUp();
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                MoveHistoryDown();
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                ExitHistoryMode();

                if (cursorPos > 0)
                {
                    cursorPos--;
                    input.Remove(cursorPos, 1);
                    RedrawInput();
                }
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                ExitHistoryMode();
                input.Insert(cursorPos, keyInfo.KeyChar);
                cursorPos++;
                RedrawInput();
            }
        }

        return input.ToString();
    }

    private static int GetDisplayWidth(string text, int tabSize = 8)
    {
        int width = 0;

        foreach (char c in text)
        {
            if (c == '\t')
            {
                width += tabSize - (width % tabSize);
            }
            else
            {
                width++;
            }
        }

        return width;
    }

    ///<summary>Print a message to the console.</summary>
    ///<param name="message">The message to print.</param>
    protected static void PrintLine(object text) => Console.WriteLine(text);

    ///<summary>Print a message to the console in a colour.</summary>
    ///<param name="message">The message to print.</param>
    ///<param name="colour">The colour to print the message in.</param>
    protected static void PrintLine(string text, Colours colour)
    {
        Console.ForegroundColor = Colour.Convert(colour);
        Console.WriteLine(text);
        Console.ResetColor();
    }

    ///<summary>Print a message to the console in gay colours.</summary>
    ///<param name="message">The message to print.</param>
    protected static void GayPrint(string message, bool newline = true)
    {
        // Create a new rainbow color
        var gayColours = new Colours[] { Colours.Red, Colours.Magenta, Colours.Blue, Colours.Cyan, Colours.Green, Colours.Yellow };

        // Index of the current color
        int colorIndex = 0;

        // Loop through each character in the message
        foreach (char c in message)
        {
            // Print the character in the next color
            Print(c.ToString(), gayColours[colorIndex]);

            // Increase the index (looping back to zero at the end)
            colorIndex++;
            if (colorIndex >= gayColours.Length) colorIndex = 0;
        }

        // Print a new line
        if (newline) PrintLine("");
    }
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