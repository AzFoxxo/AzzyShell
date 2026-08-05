namespace AzzyShell.Commands;

class Cat : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 2, allowGreaterThanLength: true))
            return (int)ErrorCode.InvalidArguments;

        // Counter
        var i = 1;

        // Check if the file exists
        foreach (var file in args.Skip(1))
        {
            // Increment counter
            i++;

            // Check if the file exists
            if (!File.Exists(file))
            {
                PrintLine($"File not found {i}", Colours.Red);
                return (int)ErrorCode.FileNotFound;
            }

            // Read the file
            string[] lines = File.ReadAllLines(file);

            // Print the file
            PrintLine($"{file}:", Colours.Blue);
            foreach (string line in lines)
            {
                PrintLine(line, Colours.Yellow);
            }
        }

        // Return success
        return (int)ErrorCode.Success;
    }

    public override string HelpString() => "Display contents of files: <file1> <file2> etc.";
}