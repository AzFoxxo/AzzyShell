namespace Heroes;

public class Common
{
    ///<summary>Print a message to the console.</summary>
    ///<param name="message">The message to print.</param>
    protected static void Print(string text) => Console.WriteLine(text);

    ///<summary>Print a message to the console in a colour.</summary>
    ///<param name="message">The message to print.</param>
    ///<param name="colour">The colour to print the message in.</param>
    protected static void Print(string text, Colours colour)
    {
        Console.ForegroundColor = Colour.Convert(colour);
        Console.WriteLine(text);
        Console.ResetColor();
    }

    ///<summary>Print a message to the console on line.</summary>
    ///<param name="message">The message to print.</param>
    protected static void PrintPrompt(string text) => Console.Write(text);

    ///<summary>Print a message to the console on line in a colour.</summary>
    ///<param name="message">The message to print.</param>
    ///<param name="colour">The colour to print the message in.</param>
    protected static void PrintPrompt(string text, Colours colour)
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
        PrintPrompt(prompt);

        // Return the input (check if the input is null or empty)
        return Console.ReadLine() ?? "";
    }

    ///<summary>Read input from the user in a colour.</summary>
    ///<param name="prompt">The prompt to display.</param>
    ///<param name="colour">The colour to display the prompt in.</param>
    ///<returns>The input from the user.</returns>
    protected static string Read(string prompt, Colours colour)
    {
        // Print the prompt
        PrintPrompt(prompt, colour);

        // Return the input (check if the input is null or empty)
        return Console.ReadLine() ?? "";
    }
}