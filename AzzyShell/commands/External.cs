namespace App.Commands;

using Heroes;
using System.Diagnostics;

public class External : Command
{
    public override int Execute(string[] args)
    {
        // Check if args given
        if (CheckArgLength(args, 1, allowGreaterThanLength: true) != 0) return 2;

        // Translate variables
        args = VariableTranslation(args);

        // Run the command in the system default shell
        if (args[0].StartsWith("!"))
        {
            // Remove the ! from the command
            string command = args[0].Remove(0, 1);

            // Try to run the command
            try
            {
                // Run the command and pause until it finishes
                ProcessStartInfo startInfo = new ProcessStartInfo(command);
                startInfo.Arguments = string.Join(" ", args.Skip(1));
                startInfo.UseShellExecute = true;
                Process.Start(startInfo).WaitForExit();

                // Return 555 if no errors
                return 555; // special code for external command success
            }
            catch (Exception e)
            {
                // Print the error
                PrintLine($"Exception occurred while running external `{command}`", Colours.Red);
                PrintLine(e.Message, Colours.Red);

                // Return error - 455 for no command
                return 455; // special code for external command failure
            }
        }

        // Return success
        return 0;
    }

    public override string Description => "Run a command in the system default shell";

    public override void PrintHelp(string[] args)
    {
        PrintLine(Description, Colours.Green);
        PrintLine($"Usage: {this.GetType().Name} [args]", Colours.Green);
        PrintLine("Args:", Colours.Green);
        PrintLine("  arg1 - Command to run*", Colours.Green);
        PrintLine("  arg2 - Arguments to pass to the command (optional)", Colours.Green);
    }
}