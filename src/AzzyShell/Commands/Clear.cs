namespace AzzyShell.Commands;

using Heroes;

public class Clear : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;
        
        // Clear the console
        // Check for errors (shouldn't be any)
        try
        {
            Console.Clear();
        }
        catch (Exception e)
        {
            // Print the error
            PrintLine($"Exception occurred while clearing the console", Colours.Red);
            PrintLine(e.Message, Colours.Red);

            // Return error - 454 for no command
            return (int)ErrorCode.ConsoleClearFailed;
        }


        // Return success
        return (int)ErrorCode.Success;
    }
}