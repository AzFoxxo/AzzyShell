namespace AzzyShell.Commands;

class Welcome : Command
{
    public override int Execute(string[] args, CommandContext context)
    {
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

        string GetVar(string key) =>
            Shell.Variables.TryGetValue(key, out var variable)
                ? variable.Value
                : string.Empty;

        Print("Welcome ");
        Print(Environment.UserName, Colours.Green);
        PrintLine("!");

        Print("You are using ");
        GayPrint($"{GetVar("shell")} ", newline: false);
        Print($"({GetVar("version")})", Colours.DarkBlue);
        Print(" by ");
        Print(GetVar("author"), Colours.Blue);
        PrintLine(".");

        PrintLine(GetVar("description"));

        Print("Type ");
        Print("'help' ", Colours.Magenta);
        PrintLine("to get started.");

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        "Display the AzzyShell welcome message.";
}