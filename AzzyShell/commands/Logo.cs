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
        Console.WriteLine(@"
     ___      ________   ________  ____    ____         _______. __    __   _______  __       __      
    /   \    |       /  |       /  \   \  /   /        /       ||  |  |  | |   ____||  |     |  |     
   /  ^  \   `---/  /   `---/  /    \   \/   /        |   (----`|  |__|  | |  |__   |  |     |  |     
  /  /_\  \     /  /       /  /      \_    _/          \   \    |   __   | |   __|  |  |     |  |     
 /  _____  \   /  /----.  /  /----.    |  |        .----)   |   |  |  |  | |  |____ |  `----.|  `----.
/__/     \__\ /________| /________|    |__|        |_______/    |__|  |__| |_______||_______||_______|                                                                                                  
");


        // Return success
        return 0;
    }
}