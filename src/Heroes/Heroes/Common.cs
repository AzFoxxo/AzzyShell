using Heroes.Attachables;
using System;

namespace Heroes;

public class Common
{
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

    ///<summary>Print a message to the console on line.</summary>
    ///<param name="message">The message to print.</param>
    protected static void Print(string text) => Console.Write(text);

    ///<summary>Print a message to the console on line in a colour.</summary>
    ///<param name="message">The message to print.</param>
    ///<param name="colour">The colour to print the message in.</param>
    protected static void Print(string text, Colours colour)
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
    protected static string ReadInline(string prompt, Colours colour = default)
    {
        // Print the prompt with colour
        Print(prompt, colour);

        var input = new System.Text.StringBuilder();
        int cursorPos = 0;

        void RedrawInput()
        {
            int currentLine = Console.CursorTop;
            int cursorLeft = prompt.Length;

            Console.SetCursorPosition(cursorLeft, currentLine);
            Console.Write(input.ToString() + " "); // Clear leftover characters
            Console.SetCursorPosition(cursorLeft + cursorPos, currentLine);
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
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (cursorPos > 0)
                {
                    cursorPos--;
                    input.Remove(cursorPos, 1);
                    RedrawInput();
                }
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                input.Insert(cursorPos, keyInfo.KeyChar);
                cursorPos++;
                RedrawInput();
            }
        }

        return input.ToString();
    }

    ///<summary>Read input from the user in a colour.</summary>
    ///<param name="prompt">The prompt to display.</param>
    ///<param name="colour">The colour to display the prompt in.</param>
    ///<returns>The input from the user.</returns>
    protected static string Read(string prompt, Colours colour)
    {
        // Print the prompt
        Print(prompt, colour);

        // Return the input (check if the input is null or empty)
        return Console.ReadLine() ?? "";
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
