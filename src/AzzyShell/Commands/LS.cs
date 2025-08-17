namespace AzzyShell.Commands;

using Heroes;

public class LS : Command
{
    public override int Execute(string[] args)
    {
       // Argument length validation
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

        // Current directory
        var currentDirectory = Directory.GetCurrentDirectory();

        // Get directory provided (if any)
        if (args.Length == 2)
        {
            // Check if directory exists
            if (!Directory.Exists(args[1]))
            {
                PrintLine("Directory does not exist!", Colours.Red);
                return (int)ErrorCode.DirectoryNotFound;
            }

            // Set current directory to provided directory
            currentDirectory = args[1];
        }

        // List all the directories in the current directory
        foreach (var dir in Directory.GetDirectories(currentDirectory))
        {
            // Print the directory name
            Print(Path.GetFileName(dir) + ", ", Colours.Blue);
        }

        // New line
        PrintLine("");

        // List all the directories in the current directory
        foreach (var file in Directory.GetFiles(currentDirectory))
        {
            var fileInfo = new FileInfo(file);
            Print($"{fileInfo.Name}, ", Colours.Green);
        }

        // New line
        PrintLine("");


        // Return success
        return (int)ErrorCode.Success;
    }
}