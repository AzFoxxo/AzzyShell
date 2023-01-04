namespace App.Commands;

using Heroes;

public class Help : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1, true) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Check if a command was given
        if (args.Length > 1)
        {
            // Check if the command exists in the command list
            var command = AzzyShell.GetInstance().CommandList.Find(x => x.Name.ToLower() == args[1].ToLower());

            // Check if the command exists
            if (command == null)
            {
                // Print error
                PrintLine($"Command `{args[1]}` does not exist", Colours.Red);

                // Return error
                return 1;
            }

            // Print the help message
            ((Command)Activator.CreateInstance(command)).PrintHelp(args);

            // Return success
            return 0;
        }

        // Print list of commands
        PrintLine("Legacy command info:");
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
        PrintLine("logo - Print the Azzy logo");
        PrintLine("hacker - Print 0 and 1 in a hacker style");
        PrintLine("gaytext - Print a message in gay colours");
        PrintLine("history - Print the command history");

        // Get a list of all commands in the current assembly (AzzyShell) and print their descriptions
        foreach (var command in AzzyShell.GetInstance().CommandList)
        {
            // Get the command instance
            var commandInstance = (Command)Activator.CreateInstance(command);

            // Print the description
            Print(commandInstance.GetType().Name.ToLower(), Colours.Red);
            Print(" - ");
            PrintLine(commandInstance.Description, Colours.Green);
        }


        // Return success
        return 0;
    }
}