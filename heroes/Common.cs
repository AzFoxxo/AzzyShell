namespace Heroes;

public class Common
{
    ///<summary>Print a message to the console.</summary>
    ///<param name="message">The message to print.</param>
    protected static void Print(string text) => Console.WriteLine(text);

    ///<summary>Print a message to the console on line.</summary>
    ///<param name="message">The message to print.</param>
    protected static void PrintPrompt(string text) => Console.Write(text);

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
}
