namespace GameCode.Commands;

public class Help : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1) != 0) return 2;

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

        // Return success
        return 0;
    }
}