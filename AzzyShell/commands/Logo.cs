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
        Print(@"     ___      ________   ________  ____    ____         _______. __    __   _______  __       __      ", Colours.DarkRed);
        Print(@"    /   \    |       /  |       /  \   \  /   /        /       ||  |  |  | |   ____||  |     |  |     ", Colours.Red);
        Print(@"   /  ^  \   `---/  /   `---/  /    \   \/   /        |   (----`|  |__|  | |  |__   |  |     |  |     ", Colours.Yellow);
        Print(@"  /  /_\  \     /  /       /  /      \_    _/          \   \    |   __   | |   __|  |  |     |  |     ", Colours.Green);
        Print(@" /  _____  \   /  /----.  /  /----.    |  |        .----)   |   |  |  |  | |  |____ |  `----.|  `----.", Colours.Blue);
        Print(@"/__/     \__\ /________| /________|    |__|        |_______/    |__|  |__| |_______||_______||_______|", Colours.Magenta);


        // Return success
        return 0;
    }
}