namespace App.Commands;

using Heroes;
using System.Diagnostics;

public class External : Command
{
    public override int Execute(string[] args)
    {
        // Ensure there is at least one argument
        if (CheckArgLength(args, 1, allowGreaterThanLength: true) != 0) return 2;

        // If the command starts with '!', it's an external command
        if (args[0].StartsWith('!'))
        {
            // Remove the '!' from the command to get the actual command name
            string command = args[0].Substring(1);

            // Ensure the command is not empty
            if (string.IsNullOrWhiteSpace(command))
            {
                PrintLine("No command specified after '!'.", Colours.Red);
                return 2;
            }

            // Prepare arguments by joining them into a single string
            string commandArguments = string.Join(" ", args.Skip(1));

            try
            {
                // Try to resolve the full path of the command if it's not a system command
                string fullCommandPath = ResolveCommandPath(command);

                if (string.IsNullOrEmpty(fullCommandPath))
                {
                    PrintLine($"Command '{command}' not found.", Colours.Red);
                    return 455;  // Special error code for command not found
                }

                // Prepare the process start info
                var startInfo = new ProcessStartInfo(fullCommandPath, commandArguments)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                // Start the process
                using (var process = Process.Start(startInfo))
                {
                    // Capture output
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    // Print the output and error to the console
                    if (!string.IsNullOrWhiteSpace(output))
                        PrintLine(output, Colours.Green);

                    if (!string.IsNullOrWhiteSpace(error))
                        PrintLine(error, Colours.Red);

                    // Wait for the process to exit and check the exit code
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        PrintLine($"Command '{command}' failed with exit code {process.ExitCode}.", Colours.Red);
                        return 455;  // Command failed
                    }
                }

                // Return 555 for success
                return 555;  // Special code for external command success
            }
            catch (Exception e)
            {
                // Catch any exceptions and print the error
                PrintLine($"Exception occurred while running external command '{command}': {e.Message}", Colours.Red);
                return 455;  // Special code for error in external command
            }
        }

        // Return success if no external command was found
        return 0;
    }

    /// <summary>
    /// Resolves the full path of a command using system utilities.
    /// </summary>
    /// <param name="command">The command name to resolve.</param>
    /// <returns>The full path of the command or null if not found.</returns>
    private string ResolveCommandPath(string command)
    {
        // Try resolving the command path using 'which' (Unix/Linux) or 'where' (Windows)
        try
        {
            string shellCommand = Environment.OSVersion.Platform == PlatformID.Win32NT ? "where" : "which";
            using (var process = Process.Start(new ProcessStartInfo(shellCommand, command) { RedirectStandardOutput = true, UseShellExecute = false }))
            {
                string output = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();

                return string.IsNullOrEmpty(output) ? null : output;
            }
        }
        catch
        {
            // Return null if the command couldn't be resolved
            return null;
        }
    }
}
