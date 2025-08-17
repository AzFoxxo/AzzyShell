namespace AzzyShell.Commands;

class PWD : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

        // Get the current directory checking for errors
        var dir = Directory.GetCurrentDirectory();
        if (dir == null)
        {
            PrintLine("Error getting current directory");
            return (int)ErrorCode.DirectoryNotFound;
        }

        // Print the current directory
        PrintLine(dir, Colours.Blue);


        // Return success
        return (int)ErrorCode.Success;
    }
}