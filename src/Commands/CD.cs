namespace AzzyShell.Commands;


class CD : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 2))
            return (int)ErrorCode.InvalidArguments;

        string targetDir = args[1];

        // Expand ~ to the user's home directory
        if (targetDir.StartsWith('~'))
            targetDir = targetDir.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

        // Check if the provided path is absolute or relative
        string fullPath = Path.IsPathRooted(targetDir)
            ? targetDir
            : Path.Combine(Directory.GetCurrentDirectory(), targetDir);

        // Check if the directory exists
        if (Directory.Exists(fullPath)) Directory.SetCurrentDirectory(fullPath);
        else
        {
            PrintLine($"Directory does not exist: {targetDir}", Colours.Red);
            return (int)ErrorCode.DirectoryNotFound;
        }

        // Return success
        return (int)ErrorCode.Success;
    }
}
