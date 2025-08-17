using Heroes;

namespace App.Commands
{
    public class Run : Command
    {
        public override int Execute(string[] args)
        {
            // Check for exactly 2 args: command name + filename
            if (CheckArgLength(args, 2) != 0)
            {
                PrintLine("Usage: run <filename>", Colours.Red);
                return 2;
            }

            string filename = args[1];

            if (!File.Exists(filename))
            {
                PrintLine($"File not found: {filename}", Colours.Red);
                return 1;
            }

            try
            {
                using var sr = new StreamReader(filename);

                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();

                    // Don't skip comments, but still handle empty lines
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Execute each command line (assuming Shell.ExecuteCommand exists)
                    int result = AzzyShell.GetInstance().ExecuteCommand(line);
                    if (result != 0)
                    {
                        PrintLine($"Command failed: {line}", Colours.Red);
                        // Decide if you want to continue or break on error
                        // break;
                    }
                }
            }
            catch (Exception ex)
            {
                PrintLine($"Error reading file: {ex.Message}", Colours.Red);
                return 1;
            }

            return 0;
        }
    }
}
