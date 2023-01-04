namespace App.Commands;

using Heroes;

public class Logo : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Print the logo
        PrintLine(@"     ___      ________   ________  ____    ____         _______. __    __   _______  __       __      ", Colours.DarkRed);
        PrintLine(@"    /   \    |       /  |       /  \   \  /   /        /       ||  |  |  | |   ____||  |     |  |     ", Colours.Red);
        PrintLine(@"   /  ^  \   `---/  /   `---/  /    \   \/   /        |   (----`|  |__|  | |  |__   |  |     |  |     ", Colours.Yellow);
        PrintLine(@"  /  /_\  \     /  /       /  /      \_    _/          \   \    |   __   | |   __|  |  |     |  |     ", Colours.Green);
        PrintLine(@" /  _____  \   /  /----.  /  /----.    |  |        .----)   |   |  |  |  | |  |____ |  `----.|  `----.", Colours.Blue);
        PrintLine(@"/__/     \__\ /________| /________|    |__|        |_______/    |__|  |__| |_______||_______||_______|", Colours.Magenta);


        // Return success
        return 0;
    }

    public override string Description => "Print the AzzyShell logo";

    public override void PrintHelp(string[] args)
    {
        PrintLine(Description, Colours.Green);
        PrintLine($"Usage: {this.GetType().Name}", Colours.Green);
    }
}