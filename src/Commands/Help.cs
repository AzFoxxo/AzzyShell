using System.Reflection;

namespace AzzyShell.Commands;

class Help : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 1, allowGreaterThanLength: true))
            return (int)ErrorCode.InvalidArguments;

        if (args.Length < 2)
        {
            // Print list of commands
            PrintLine("Commands:");
            PrintLine("- quit - Quit the app");
            PrintLine("- help - Show this help message");
            PrintLine("- ls - List all files and directories in the current directory");
            PrintLine("- pwd - Print the current directory");
            PrintLine("- clear - Clear the console");
            PrintLine("- cd - Change the current directory");
            PrintLine("- touch - Update timetamp of a file or create a new file");
            PrintLine("- mkdir - Create a new directory");
            PrintLine("- remove - Remove a file or directory");
            PrintLine("- cat - Print the contents of a file");
            PrintLine("- log - Log a message to the console");
            PrintLine("- fizzbuzz - Print the FizzBuzz sequence up to a given number");
            PrintLine("- set - Set a variable");
            PrintLine("- vars - List all variables");
            PrintLine("- logo - Print the Azzy logo");
            PrintLine("- hacker - Print 0 and 1 in a hacker style");
            PrintLine("- gaytext - Print a message in gay colours");
            PrintLine("- history - Print the command history");
            PrintLine("- run - Run a AzzyShell Script file line-by-line");
            PrintLine("- alias - Create/update and view aliases");
        }
        else
        {
            // Perform foreach
            foreach (var commandName in args.Skip(1))
            {
                var type = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(t =>
                        t.Namespace == "AzzyShell.Commands" &&
                        t.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));

                if (type is null)
                {
                    PrintLine($"Command '{commandName}' not found.", Colours.Red);
                    continue;
                }

                if (Activator.CreateInstance(type) is Command command)
                {
                    PrintLine($"{commandName}: {command.HelpString()}");
                }
                else
                {
                    PrintLine($"Failed to load command '{commandName}'.", Colours.Red);
                }
            }
        }

        // Return success
        return (int)ErrorCode.Success;
    }

    public override string HelpString() => "Display's this help info `help` or `help <command> <command> to display help for specific commands`";
}