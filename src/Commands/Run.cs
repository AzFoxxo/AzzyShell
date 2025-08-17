namespace AzzyShell.Commands;

class Run : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 2))
            return (int)ErrorCode.InvalidArguments;

        string filename = args[1];

        if (!File.Exists(filename))
        {
            PrintLine($"File not found: {filename}", Colours.Red);
            return (int)ErrorCode.FileNotFound;
        }

        try
        {
            using var sr = new StreamReader(filename);

            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();

                // Don't skip comments, but still handle empty lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Execute each command line (assuming Shell.ExecuteCommand exists)
                int result = Shell.ExecuteCommand(line);
                if (result != 0)
                {
                    PrintLine($"Command failed: {line}", Colours.Red);
                    // Decide if you want to continue or break on error
                    // break;
                }
            }
        }
        catch (Exception ex)
        {
            PrintLine($"Error reading file: {ex.Message}", Colours.Red);
            return (int)ErrorCode.FileReadError;
        }

        return (int)ErrorCode.Success;
    }
}
