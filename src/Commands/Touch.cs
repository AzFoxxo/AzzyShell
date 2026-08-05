namespace AzzyShell.Commands;

class Touch : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation (allowing multiple files)
        if (!IsArgumentLengthValid(args, 2, allowGreaterThanLength: true))
            return (int)ErrorCode.InvalidArguments;

        // Iterate over all files
        foreach (var filePath in args.Skip(1))
        {
            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Update timestamps
                File.SetLastAccessTime(filePath, DateTime.Now);
                File.SetLastWriteTime(filePath, DateTime.Now);
            }
            else
            {
                // Create the file
                using (File.Create(filePath)) { }
            }
        }

        // Return success
        return (int)ErrorCode.Success;
    }

    public override string HelpString() => "Create/update file time access signature takes <arg1> <arg2> etc.";
}
