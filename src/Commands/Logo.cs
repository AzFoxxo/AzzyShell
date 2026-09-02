namespace AzzyShell.Commands;

class Logo : Command
{
    public override int Execute(string[] args, CommandContext context)
    {
        if (!IsArgumentLengthValid(args, 1))
            return (int)ErrorCode.InvalidArguments;

        PrintLine(@"     ___      ________   ________  ____    ____         _______. __    __   _______  __       __      ", Colours.DarkRed);
        PrintLine(@"    /   \    |       /  |       /  \   \  /   /        /       ||  |  |  | |   ____||  |     |  |     ", Colours.Red);
        PrintLine(@"   /  ^  \    `---/  /   `---/  /    \   \/   /        |   (----`|  |__|  | |  |__   |  |     |  |     ", Colours.Yellow);
        PrintLine(@"  /  /_\  \     /  /       /  /      \_    _/          \   \    |   __   | |   __|  |  |     |  |     ", Colours.Green);
        PrintLine(@" /  _____  \   /  /----.  /  /----.    |  |        .----)   |   |  |  |  | |  |____ |  `----.|  `----.", Colours.Blue);
        PrintLine(@"/__/     \__\ /________| /________|    |__|        |_______/    |__|  |__| |_______||_______||_______|", Colours.Magenta);

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        "Display the AzzyShell ASCII logo.";
}