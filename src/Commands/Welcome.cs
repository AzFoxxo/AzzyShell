namespace AzzyShell.Commands;

class Welcome : Command
{
    public override int Execute(string[] args)
    {
        // Argument length validation
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

        // Helper local function to get variable value safely
        string GetVar(string key) => Shell.Variables.TryGetValue(key, out var v) ? v.Value : "";

        // Welcome <username>!
        Print("Welcome ");
        Print(Environment.UserName, Colours.Green);
        PrintLine("!");

        // You are using <shell> <version> by <author>.
        Print("You are using ");
        GayPrint($"{GetVar("shell")} ", newline: false);
        Print($"({GetVar("version")})", Colours.DarkBlue);
        Print(" by ");
        Print($"{GetVar("author")}", Colours.Blue);
        PrintLine(".");

        // Description
        PrintLine(GetVar("description"));

        // Basic getting started info
        Print("Type");
        Print(" 'help' ", Colours.Magenta);
        PrintLine("to get started.");

        // Return success
        return (int)ErrorCode.Success;;
    }
}
