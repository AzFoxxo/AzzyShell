namespace AzzyShell.Commands;

class Touch : Command
{
    public override int Execute(string[] args)
    {
        if (!IsArgumentLengthValid(args, 2, allowGreaterThanLength: true))
            return (int)ErrorCode.InvalidArguments;

        foreach (var filePath in args.Skip(1))
        {
            if (File.Exists(filePath))
            {
                DateTime now = DateTime.Now;

                File.SetLastAccessTime(filePath, now);
                File.SetLastWriteTime(filePath, now);
            }
            else
            {
                using (File.Create(filePath))
                {
                }
            }
        }

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        """
        Create a file or update its timestamps:

        `touch <file1> [file2] ...`

        If a file exists:
            Updates access and modification timestamps.

        If a file does not exist:
            Creates an empty file.

        Examples:
            touch file.txt
            touch file1.txt file2.txt
        """;
}