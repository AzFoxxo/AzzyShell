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
            string script = File.ReadAllText(filename);
            return Shell.ExecuteCommand(script);
        }
        catch (Exception ex)
        {
            PrintLine($"Error reading file: {ex.Message}", Colours.Red);
            return (int)ErrorCode.FileReadError;
        }
    }

    public override string HelpString() => "Run an AzzyShell script: <file>";
}
