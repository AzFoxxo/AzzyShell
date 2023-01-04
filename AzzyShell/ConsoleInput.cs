using System;
using Heroes;
// TODO: Add a command history viewing from the up and down arrow keys
// TODO: Add prompt support
// TODO: Add colour support
namespace ConsoleApp
{
    public class ConsoleInput : Hero
    {
        public static string ReadLine(string prompt = "", Colours colour=Colours.White)
        {
            #if false
            // Display the prompt message if provided
            if (!string.IsNullOrEmpty(prompt))
            {
                Print(prompt, colour);

                // Add a space after the prompt
                Console.Write(" ");

                // Move the cursor back to the start of the line

            }
            #endif

            string input = "";
            int cursorPos = 0;
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Handle left arrow key
                if (key.Key == ConsoleKey.LeftArrow && cursorPos > 0)
                {
                    cursorPos--;
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                }
                // Handle right arrow key
                else if (key.Key == ConsoleKey.RightArrow && cursorPos < input.Length)
                {
                    cursorPos++;
                    Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                }
                // Handle backspace
                else if (key.Key == ConsoleKey.Backspace && cursorPos > 0)
                {
                    input = input.Substring(0, cursorPos - 1) + input.Substring(cursorPos);
                    cursorPos--;
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    Console.Write(" \b");
                }
                // Handle delete
                else if (key.Key == ConsoleKey.Delete && cursorPos < input.Length)
                {
                    input = input.Substring(0, cursorPos) + input.Substring(cursorPos + 1);
                    Console.Write(" \b");
                }
                // Handle newline
                else if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                // Add any other key to the input string
                else if (key.Key != ConsoleKey.Backspace)
                {
                    // Append the character to the input string if it is a printable character
                    if (!char.IsControl(key.KeyChar))
                    {
                        input = input.Substring(0, cursorPos) + key.KeyChar + input.Substring(cursorPos);
                        cursorPos++;
                    }

                    // Clear the line and write the updated input string
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(input);

                    // Move the cursor back to the correct position
                    Console.SetCursorPosition(cursorPos, Console.CursorTop);
                }
            } while (true);

            return input;
        }
    }
}
