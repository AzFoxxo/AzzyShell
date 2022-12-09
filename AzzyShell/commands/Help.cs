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
        PrintLine("Commands:");
        PrintLine("quit - Quit the app");
        PrintLine("help - Show this help message");
        PrintLine("ls - List all files and directories in the current directory");
        PrintLine("pwd - Print the current directory");
        PrintLine("clear - Clear the console");
        PrintLine("cd - Change the current directory");
        PrintLine("touch - Create a new file");
        PrintLine("mkdir - Create a new directory");
        PrintLine("remove - Remove a file or directory");
        PrintLine("cat - Print the contents of a file");
        PrintLine("log - Log a message to the console");
        PrintLine("fizzbuzz - Print the FizzBuzz sequence up to a given number");
        PrintLine("set - Set a variable");
        PrintLine("vars - List all variables");
        PrintLine("logo - Print the Azzy logo");
        PrintLine("hacker - Print 0 and 1 in a hacker style");
        PrintLine("gaytext - Print a message in gay colours");
        PrintLine("history - Print the command history");


        // Return success
        return 0;
    }
}