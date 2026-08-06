namespace AzzyShell.Commands;

class Remove : Command
{
    public override int Execute(string[] args)
    {
        if (!IsArgumentLengthValid(args, 2, allowGreaterThanLength: true))
            return (int)ErrorCode.InvalidArguments;

        foreach (var path in args.Skip(1))
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            else
            {
                PrintLine($"Path not found: {path}", Colours.Red);
                return (int)ErrorCode.PathNotFound;
            }
        }

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        """
        Remove files or directories:

        `remove <path1> [path2] ...`

        Files are deleted directly.
        Directories are deleted recursively.

        Examples:
            remove file.txt
            remove file1.txt file2.txt
            remove old_folder
        """;
}