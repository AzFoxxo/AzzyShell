namespace App.Commands;

using Heroes;

public class Help : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Print list of commands
        Print("Commands:");
        Print("quit - Quit the app");
        Print("help - Show this help message");
        Print("ls - List all files and directories in the current directory");
        Print("pwd - Print the current directory");
        Print("clear - Clear the console");
        Print("cd - Change the current directory");
        Print("touch - Create a new file");
        Print("mkdir - Create a new directory");
        Print("remove - Remove a file or directory");
        Print("cat - Print the contents of a file");
        Print("log - Log a message to the console");
        Print("fizzbuzz - Print the FizzBuzz sequence up to a given number");
        Print("set - Set a variable");
        Print("vars - List all variables");
        Print("logo - Print the Azzy logo");
        Print("hacker - Print 0 and 1 in a hacker style");
        Print("gaytext - Print a message in gay colours");
        Print("history - Print the command history");


        // Return success
        return 0;
    }
}