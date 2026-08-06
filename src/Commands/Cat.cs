namespace AzzyShell.Commands;

class Cat : Command
{
    public override int Execute(string[] args)
    {
        if (!IsArgumentLengthValid(args, 2, allowGreaterThanLength: true))
            return (int)ErrorCode.InvalidArguments;

        foreach (string file in args.Skip(1))
        {
            if (!File.Exists(file))
            {
                PrintLine($"File not found: {file}", Colours.Red);
                return (int)ErrorCode.FileNotFound;
            }

            try
            {
                PrintLine($"{file}:", Colours.Blue);

                foreach (string line in File.ReadLines(file))
                {
                    PrintLine(line, Colours.Yellow);
                }
            }
            catch (Exception ex)
            {
                PrintLine($"Failed to read '{file}': {ex.Message}", Colours.Red);
                return (int)ErrorCode.FileReadError;
            }
        }

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        """
        Display the contents of files:

        `cat <file1> [file2] [file3]`

        Supports displaying multiple files in order.
        """;
}