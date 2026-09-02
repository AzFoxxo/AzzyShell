namespace AzzyShell.Commands;

class Cd : Command
{
    public override int Execute(string[] args, CommandContext context)
    {
        // Allow `cd` with no arguments and `cd <dir>` with one argument
        if (args.Length > 2)
            return (int)ErrorCode.InvalidArguments;

        string targetDir = args.Length == 1 ? "~" : args[1];

        if (targetDir == "-")
        {
            if (Shell.PreviousDirectory is null)
            {
                PrintLine("No previous directory.", Colours.Red);
                return (int)ErrorCode.DirectoryNotFound;
            }

            targetDir = Shell.PreviousDirectory;
        }

        targetDir = ResolvePath(targetDir);

        if (!Directory.Exists(targetDir))
        {
            PrintLine($"Directory does not exist: {targetDir}", Colours.Red);
            return (int)ErrorCode.DirectoryNotFound;
        }

        try
        {
            string previousDirectory = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(targetDir);

            Shell.PreviousDirectory = previousDirectory;
        }
        catch (Exception ex)
        {
            PrintLine($"Failed to change directory: {ex.Message}", Colours.Red);
            return (int)ErrorCode.ExecutionFailure;
        }

        return (int)ErrorCode.Success;
    }

    private static string ResolvePath(string path)
    {
        if (path == "~" || path.StartsWith("~/"))
        {
            string home = Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile
            );

            return path == "~"
                ? home
                : Path.Combine(home, path[2..]);
        }

        return path;
    }

    public override string HelpString() =>
        """
        Change the current directory:

        `cd`
            Change to the user's home directory.

        `cd <dir>`
            Change to a directory.

        `cd ~`
            Change to the user's home directory.

        `cd -`
            Return to the previous directory.
        """;
}