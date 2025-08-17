namespace AzzyShell;

using System.Reflection;
using System.Text.RegularExpressions;
using AzzyShell.Commands;

partial class Azzy : HeroesPatch
{
    private const string version = "2.3.0";
    private const string shell = "Azzy";
    private const string author = "Az Foxxo";
    private const string description = "A lightweight shell environment written in C#";
    private const string prompt = "$";

    int returnedCode = 0;
    private static Azzy? instance;

    public Dictionary<string, ShellVariable> Variables { get; private set; } = [];

    private string[] args = [];

    public string historyFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".history.azzy");
    private static readonly string[] separator = ["&&", "\n"];

    public bool DeleteVariable(string name) => Variables.Remove(name);

    [GeneratedRegex("@DOUBLE@")]
    private static partial Regex DoubleQuoteRegex();

    [GeneratedRegex("@AND@")]
    private static partial Regex AndRegex();

    [GeneratedRegex("@NEWLINE@")]
    private static partial Regex NewlineRegex();

    [GeneratedRegex("@AT@")]
    private static partial Regex AtRegex();

    [GeneratedRegex("@TAB@")]
    private static partial Regex TabRegex();

    [GeneratedRegex("@HASH@")]
    private static partial Regex HashRegex();

    [GeneratedRegex(@"""[^""]*""|[^ ]+")]
    public static partial Regex ArgumentSplitter();

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

    public string? GetVariable(string name)
    {
        return Variables.TryGetValue(name, out var variable) ? variable.Value : null;
    }

    public bool VariableExists(string name) => Variables.ContainsKey(name);

    // Add shell variables and store a reference to the shell
    public Azzy()
    {
        SetVariable("version", version, "String");
        SetVariable("shell", shell, "String");
        SetVariable("author", author, "String");
        SetVariable("description", description, "String");
        SetVariable("prompt", prompt, "String");
        SetVariable("returnedCode", returnedCode.ToString(), "Int");
        SetVariable("colour", "Cyan", "String");

        instance = this;

        string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string initFilePath = Path.Combine(homeDirectory, ".azzyshell_init.ass");

        // Check if the config file exists
        if (File.Exists(initFilePath))
        {
            // If the config file exists, run it using the "run" command
            returnedCode = new Run().Execute(["run", initFilePath]);

            if (returnedCode != 0)
            {
                // Handle any errors from the initialization script
                PrintLine($"Error occurred while running {initFilePath}.", Colours.Red);
                return;
            }
        }
        else
        {
            // Run default commands if the initialization file does not exist
            returnedCode = new Clear().Execute(["clear"]);
            returnedCode = new Welcome().Execute(["welcome"]);
            returnedCode = new Logo().Execute(["logo"]);
        }

        // Check for history file and create if it doesn't exist
        if (!File.Exists(historyFile))
        {
            File.Create(historyFile).Close();
        }
    }

    public int ExecuteCommand(string input)
    {
        // Split the input into several commands (support && and new lines)
        var commands = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        int lastReturnedCode = 0;

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

            // Update returnedCode variable before running
            UpdateReturnedCodeVariable(lastReturnedCode);

            // Set args as a field or property if your CommandSwitch depends on it
            this.args = args;

            lastReturnedCode = CommandSwitch();

            // Update returnedCode variable after running
            UpdateReturnedCodeVariable(lastReturnedCode);
        }

        return lastReturnedCode;
    }


    private void UpdateReturnedCodeVariable(int code) => SetVariable("returnedCode", code.ToString(), "Int");

    public void Tick()
    {
        // Get the input
        var input = GetCurrentLine();

        // Execute the command
        ExecuteCommand(input);
    }

    private string GetCurrentLine()
    {
        // Get colour and prompt from variables
        string colourStr = GetVariable("colour") ?? "green";
        string promptStr = GetVariable("prompt") ?? "~";

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


    public static Azzy GetInstance() => instance!;

    public int CommandSwitch()
    {
        // Resolve arguments
        string[] resolvedCommand;
        try
        {
            resolvedCommand = Command.VariableResolution(args);
        }
        catch (Exception ex)
        {
            PrintLine($"[Error] {ex.Message}", Colours.Red);
            return (int)ErrorCode.ExecutionFailure;
        }

        if (resolvedCommand.Length == 0)
            return (int)ErrorCode.InvalidArguments;

        string commandKey = resolvedCommand[0];

        // Check for built-in command (match class name)
        var commandType = Assembly.GetExecutingAssembly()
            .GetTypes()
            .FirstOrDefault(t =>
                typeof(Command).IsAssignableFrom(t) &&
                !t.IsAbstract &&
                string.Equals(t.Name, commandKey, StringComparison.OrdinalIgnoreCase)
            );

        if (commandType != null)
        {
            // Check if the instance is null before executing
            if (Activator.CreateInstance(commandType) is Command commandInstance)
            {
                return commandInstance.Execute(resolvedCommand);
            }
            else
            {
                // Handle the case where the instance couldn't be created
                PrintLine("Failed to create command instance.", Colours.Red);
                return (int)ErrorCode.ExecutionFailure;
            }
        }


        // Fallback to external command using program_path variable
        string pathVar = GetVariable("program_path") ?? "/bin:/usr/bin";
        var paths = pathVar.Split(':', StringSplitOptions.RemoveEmptyEntries);

        string? executablePath = null;

        foreach (var path in paths)
        {
            var candidate = Path.Combine(path, commandKey);
            if (File.Exists(candidate))
            {
                executablePath = candidate;
                break;
            }
        }

        if (executablePath == null)
        {
            PrintLine($"Failed to find the command `{commandKey}` in program_path", Colours.Red);
            return 1;
        }

        try
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = executablePath;
            process.StartInfo.Arguments = string.Join(' ', resolvedCommand.Skip(1));
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(output))
                Console.WriteLine(output);

            if (!string.IsNullOrEmpty(error))
                Console.Error.WriteLine(error);

            return process.ExitCode;
        }
        catch (Exception ex)
        {
            PrintLine($"[Error] Failed to run external command: {ex.Message}", Colours.Red);
            return 1;
        }
    }
}
