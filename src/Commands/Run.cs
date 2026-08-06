namespace AzzyShell.Commands;

class Run : Command
{
    public override int Execute(string[] args)
    {
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
        catch (IOException ex)
        {
            PrintLine($"Error reading script: {ex.Message}", Colours.Red);
            return (int)ErrorCode.FileReadError;
        }
        catch (Exception ex)
        {
            PrintLine($"Error executing script: {ex.Message}", Colours.Red);
            return (int)ErrorCode.ExecutionFailure;
        }
    }

    public override string HelpString() =>
        """
        Execute an AzzyShell script in the current shell session:

        `run <file>`

        Scripts can contain:
            - Commands
            - Variables
            - Aliases
            - Conditionals (V3)

        Examples:
            run ~/.azzyshell_init.ass
            run setup.ass
        """;
}