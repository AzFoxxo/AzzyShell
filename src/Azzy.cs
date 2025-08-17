namespace AzzyShell;

using System.Reflection;
using System.Text.RegularExpressions;
using AzzyShell.Commands;

partial class Azzy : HeroesPatch
{
    // Singleton
    private static Azzy? instance;

    // Constants
    private const string version = "2.4.1";
    private const string shell = "Azzy";
    private const string author = "Az Foxxo";
    private const string description = "A lightweight shell environment written in C#.";
    private const string prompt = "$";

    // Separators
    private static string[] separator = ["&&", "\n"];

    // Substitutes
    public Dictionary<string, ShellVariable> Variables { get; private set; } = [];
    public Dictionary<string, string> Aliases { get; private set; } = [];

    // Command arguments
    private string[] commandArguments = [];

    // History file path
    public string historyFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".history.azzy");

    // Regex
    [GeneratedRegex("@DOUBLE@")] private static partial Regex DoubleQuoteRegex();

    [GeneratedRegex("@AND@")] private static partial Regex AndRegex();

    [GeneratedRegex("@NEWLINE@")] private static partial Regex NewlineRegex();

    [GeneratedRegex("@AT@")] private static partial Regex AtRegex();

    [GeneratedRegex("@TAB@")] private static partial Regex TabRegex();

    [GeneratedRegex("@HASH@")] private static partial Regex HashRegex();

    [GeneratedRegex(@"""[^""]*""|[^ ]+")] public static partial Regex ArgumentSplitter();

    /// <summary>
    /// Constructor - shell configuration and initialisation 
    /// </summary>
    public Azzy()
    {
        SetVariable("version", version, "String");          // Version variable
        SetVariable("shell", shell, "String");              // Shell name variable
        SetVariable("author", author, "String");            // Author name variable
        SetVariable("description", description, "String");  // Description variable
        SetVariable("prompt", prompt, "String");            // Prompt text variable
        SetVariable("status", "0", "Int");                  // Status code variable
        SetVariable("colour", "Cyan", "String");            // Prompt colour variable

        instance = this;

        string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string initFilePath = Path.Combine(homeDirectory, ".azzyshell_init.ass");

        // Check if the config file exists
        if (File.Exists(initFilePath))
        {
            // If the config file exists, run it using the "run" command
            ExecuteAndCapture($"run {initFilePath}");
            if (GetVariable("status") != "0")
            {
                // Handle any errors from the initialization script
                PrintLine($"Error occurred while running {initFilePath}.", Colours.Red);
                return;
            }
        }
        else
        {
            // Run default commands if the initialization file does not exist
            ExecuteAndCapture("clear");
            ExecuteAndCapture("welcome");
            ExecuteAndCapture("logo");
        }

        // Check for history file and create if it doesn't exist
        if (!File.Exists(historyFile))
        {
            File.Create(historyFile).Close();
        }
    }

    /// <summary>
    /// Delete a variable 
    /// </summary>
    /// <param name="name">variable name to delete</param>
    /// <returns>Return if the deletion was successful</returns>
    public bool DeleteVariable(string name) => Variables.Remove(name);

    /// <summary>
    /// Set (update or create) a given variable in the shell
    /// </summary>
    /// <param name="name">variable name</param>
    /// <param name="value">value</param>
    /// <param name="type">type</param>
    public void SetVariable(string name, string value, string type = "String")
    {
        if (VariableExists(name))
        {
            Variables[name].Value = value;
            Variables[name].Type = type;
        }
        else
        {
            Variables[name] = new ShellVariable(name, value, type);
        }
    }

    /// <summary>
    /// Get the value of the shell variable
    /// </summary>
    /// <param name="name">variable name</param>
    /// <returns>value of the variable</returns>
    public string? GetVariable(string name)
    {
        return Variables.TryGetValue(name, out var variable) ? variable.Value : null;
    }

    /// <summary>
    /// Check if a shell variable exists
    /// </summary>
    /// <param name="name">variable name</param>
    /// <returns>true if it exists, else false</returns>
    public bool VariableExists(string name) => Variables.ContainsKey(name);

    /// <summary>
    /// Execute the command and capture the status code in the status variable
    /// </summary>
    /// <param name="input">Raw command after alias resolution</param>
    public void ExecuteAndCapture(string input) => SetVariable("status", $"{(int)ExecuteCommand(input)}", "Int");

    /// <summary>
    /// Execute raw command (requires capturing status code)
    /// </summary>
    /// <param name="input">Raw command after alias resolution</param>
    /// <returns>Status code (int)</returns>
    public int ExecuteCommand(string input)
    {
        // Apply alias to input
        input = ResolveAliases(input);

        // Split the input into several commands (support && and new lines)
        var commands = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        foreach (string command in commands)
        {
            string trimmedCommand = command.Trim();

            // Skip the line if it starts with # (comment)
            if (trimmedCommand.StartsWith('#'))
                continue;

            // Remove inline comments (anything after #)
            int commentIndex = trimmedCommand.IndexOf('#');
            if (commentIndex >= 0)
            {
                trimmedCommand = trimmedCommand[..commentIndex].Trim();
            }

            #region @Replaces@
            // Generate unique GUIDs for each placeholder
            string doubleQuote = Guid.NewGuid().ToString();
            string and = Guid.NewGuid().ToString();
            string newline = Guid.NewGuid().ToString();
            string at = Guid.NewGuid().ToString();
            string tab = Guid.NewGuid().ToString();
            string hash = Guid.NewGuid().ToString();

            // Replace placeholders with generated GUIDs using regex
            trimmedCommand = DoubleQuoteRegex().Replace(trimmedCommand, doubleQuote);
            trimmedCommand = AndRegex().Replace(trimmedCommand, and);
            trimmedCommand = NewlineRegex().Replace(trimmedCommand, newline);
            trimmedCommand = AtRegex().Replace(trimmedCommand, at);
            trimmedCommand = TabRegex().Replace(trimmedCommand, tab);
            trimmedCommand = HashRegex().Replace(trimmedCommand, hash);
            #endregion

            // Split the command into arguments based on spaces and quoted substrings
            var args = ArgumentSplitter().Matches(trimmedCommand)
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray();

            // Clean up the arguments by reverting the GUID replacements
            for (int i = 0; i < args.Length; i++)
            {
                // Replace GUIDs with their corresponding special characters
                args[i] = args[i].Replace("\"", ""); // Remove extra quotes
                args[i] = args[i].Replace(doubleQuote, "\"");  // Revert GUID to double quote
                args[i] = args[i].Replace(and, "&&");           // Revert GUID to &&
                args[i] = args[i].Replace(newline, "\n");       // Revert GUID to newline
                args[i] = args[i].Replace(at, "@");             // Revert GUID to @
                args[i] = args[i].Replace(tab, "\t");           // Revert GUID to tab
                args[i] = args[i].Replace(hash, "#");           // Revert GUID to hash
            }

            // Skip if no arguments or only whitespace
            if (args.Length < 1 || string.IsNullOrWhiteSpace(args[0]))
                continue;

            // Set args as a field or property if your CommandSwitch depends on it
            this.commandArguments = args;

            // Return the result of CommandSwitch() directly
            return CommandSwitch();
        }

        return (int)ErrorCode.Success;
    }

    /// <summary>
    /// Tick - Read input and execute input
    /// </summary>
    public void Tick()
    {
        // Get the input
        var input = Prompt();

        // Execute the command
        ExecuteAndCapture(input);
    }

    /// <summary>
    /// Displays prompt, capturing the prompt once enter his hit
    /// </summary>
    /// <returns>Raw prompt input</returns>
    private string Prompt()
    {
        // Get colour and prompt from variables
        string colourStr = GetVariable("colour") ?? "green";
        string promptStr = GetVariable("prompt") ?? "~";

        // Add status code if non-zero
        var status = GetVariable("status");
        if (status != "0")
            promptStr = $"({status}) {promptStr}";

        // Convert string colour to Heroes enum
        if (!Enum.TryParse(colourStr, true, out Colours heroesColour))
        {
            heroesColour = Colours.Red; // Fallback on failure
        }

        // Read the current line with coloured prompt
        string input = ReadInline(promptStr + " ", heroesColour);

        // Append to history
        File.AppendAllText(historyFile, input + "\n");

        return input;
    }

    /// <summary>
    /// Get the reference to the AzzyShell
    /// </summary>
    /// <returns>Reference to AzzyShell</returns>
    public static Azzy GetInstance() => instance!;

    /// <summary>
    /// Execute the given command, performing variable resolution
    /// </summary>
    /// <returns>status code</returns>
    public int CommandSwitch()
    {
        // Argument resolution
        string[] resolvedCommand;
        try
        {
            resolvedCommand = Command.VariableResolution(commandArguments);
        }
        catch (Exception ex)
        {
            PrintLine($"[Error] {ex.Message}", Colours.Red);
            return (int)ErrorCode.ExecutionFailure;
        }

        if (resolvedCommand.Length == 0)
            return (int)ErrorCode.InvalidArguments;

        // Get the command
        var command = resolvedCommand[0];

        // Check for built-in commands
        var commandType = Assembly.GetExecutingAssembly()
            .GetTypes()
            .FirstOrDefault(t =>
                typeof(Command).IsAssignableFrom(t) &&
                !t.IsAbstract &&
                string.Equals(t.Name, command, StringComparison.OrdinalIgnoreCase)
            );

        if (commandType is not null)
        {
            // Execute command if null check passed
            if (Activator.CreateInstance(commandType) is Command commandInstance)
            {
                return commandInstance.Execute(resolvedCommand);
            }
            else
            {
                // Log null check failure
                PrintLine("Failed to create command instance.", Colours.Red);
                return (int)ErrorCode.ExecutionFailure;
            }
        }

        // Command not found, check path for external commands
        string pathVar = GetVariable("path") ?? "/bin:/usr/bin"; // Query path
        var paths = pathVar.Split(':', StringSplitOptions.RemoveEmptyEntries); // Get paths

        string? executablePath = null;

        foreach (var path in paths)
        {
            var candidate = Path.Combine(path, command);
            if (File.Exists(candidate))
            {
                executablePath = candidate;
                break;
            }
        }

        if (executablePath is null)
        {
            PrintLine($"Failed to find the command `{command}` in path", Colours.Red);
            return (int)ErrorCode.CommandNotFound;
        }

        try
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = executablePath;
            process.StartInfo.Arguments = string.Join(' ', resolvedCommand.Skip(1));

            // Check if command is interactive
            bool isInteractive = IsCommandInteractive();

            if (isInteractive)
            {
                // Interactive commands
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = false;
            }
            else
            {
                // Non-interactive commands
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
            }

            process.Start();

            if (!process.StartInfo.UseShellExecute)
            {
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                    Console.WriteLine(output);

                if (!string.IsNullOrEmpty(error))
                    Console.Error.WriteLine(error);
            }
            else
            {
                // Wait for process to terminate
                process.WaitForExit();
            }

            return process.ExitCode;
        }
        catch (Exception ex)
        {
            PrintLine($"[Error] Failed to run external command: {ex.Message}", Colours.Red);
            return (int)ErrorCode.CommandNotFound;
        }
    }

    /// <summary>
    /// Is the command running interactive
    /// </summary>
    /// <returns>True if command is interactive, else false</returns>
    private bool IsCommandInteractive() => !Console.IsInputRedirected && !Console.IsOutputRedirected;

    /// <summary>
    /// Return the command with the alias resolved
    /// </summary>
    /// <param name="command">Unresolved command alias</param>
    /// <returns>Resolved alias and command</returns>
    private string ResolveAliases(string command)
    {
        // Check if the alias exists
        if (Aliases.ContainsKey(command))
        {
            // Substitute alias
            string aliasValue = Aliases[command];
            return aliasValue;
        }

        // No alias found
        return command;
    }

}
