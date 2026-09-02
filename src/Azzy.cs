namespace AzzyShell;

using System.Reflection;
using AzzyShell.Commands;

partial class Azzy
{
    // Singleton
    private static Azzy? instance;

    // Constants
    private const string version = "3.1.0";
    private const string shell = "Azzy";
    private const string author = "Az Foxxo";
    private const string description = "A lightweight shell environment written in C#.";
    private const string prompt = "$";

    // Separators
    private static readonly string[] separator = ["&&", "\n"];
    private const int AliasRecursionLimit = 20;

    // Substitutes
    public Dictionary<string, ShellVariable> Variables { get; private set; } = [];
    public Dictionary<string, string> Aliases { get; private set; } = [];

    // Command arguments
    private string[] commandArguments = [];

    // Command history cache
    private readonly List<string> historyEntries = [];

    // History file path
    public string historyFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".history.azzy");

    // Previous directory for 'cd -' command
    private string? previousDirectory;

    public string? PreviousDirectory
    {
        get => previousDirectory;
        set => previousDirectory = value;
    }

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
                Console.Error.WriteLine($"Error occurred while running {initFilePath}.");
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

        historyEntries.AddRange(File.ReadAllLines(historyFile));
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
        if (!Variables.TryGetValue(name, out var variable))
            return null;

        return variable.Type == "String"
            ? ScriptText.UnwrapLiteral(variable.Value)
            : variable.Value;
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
    public void ExecuteAndCapture(string input) => SetVariable("status", $"{ExecuteCommand(input)}", "Int");

    internal IReadOnlyList<string> HistoryEntries => historyEntries;

    internal void AppendHistoryEntry(string entry)
    {
        if (string.IsNullOrWhiteSpace(entry))
            return;

        historyEntries.Add(entry);
        File.AppendAllText(historyFile, entry + "\n");
    }

    /// <summary>
    /// Resolve script tokens after parsing and alias expansion.
    /// </summary>
    /// <param name="args">Tokenized command arguments</param>
    /// <returns>Resolved arguments</returns>
    internal string[] ResolveScriptTokens(string[] args)
    {
        return ScriptArgumentResolver.Resolve(this, args);
    }

    /// <summary>
    /// Execute raw command (requires capturing status code)
    /// </summary>
    /// <param name="input">Raw command after alias resolution</param>
    /// <returns>Status code (int)</returns>
    public int ExecuteCommand(string input)
    {
        return new ScriptEngine(this).Execute(input);
    }

    /// <summary>
    /// Execute a parsed command array.
    /// </summary>
    /// <param name="inputArgs">Command and arguments</param>
    /// <param name="standardInput">Optional stdin content</param>
    /// <returns>Status code</returns>
    public int ExecuteCommandArray(
        string[]? inputArgs = null,
        string? standardInput = null,
        bool forceCaptureStreams = false,
        CommandContext? context = null)
    {
        var resolvedCommand = inputArgs ?? commandArguments;

        if (resolvedCommand.Length == 0)
            return (int)ErrorCode.InvalidArguments;

        // Resolve command aliases first
        var command = resolvedCommand[0];
        int aliasDepth = 0;

        while (!command.Contains('/') && Aliases.TryGetValue(command, out var alias))
        {
            aliasDepth++;

            // Prevent infinite alias recursion
            if (aliasDepth > AliasRecursionLimit)
            {
                Console.Error.WriteLine("Alias recursion limit exceeded.");
                return (int)ErrorCode.AliasRecursionLimitExceeded;
            }

            var aliasArgs = ScriptTokenizer.Tokenize(alias)
                .Where(t => t.Kind is ScriptTokenKind.Word or ScriptTokenKind.String)
                .Select(t => t.Value)
                .ToArray();

            resolvedCommand = [.. aliasArgs, .. resolvedCommand.Skip(1)];
            command = resolvedCommand[0];
        }

        try
        {
            // Resolve variables and paths after alias expansion.
            resolvedCommand = ResolveScriptTokens(resolvedCommand);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[Error] {ex.Message}");
            return (int)ErrorCode.ExecutionFailure;
        }

        command = resolvedCommand[0];

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
                var commandContext = new CommandContext(
                    standardInput is null ? Console.In : new StringReader(standardInput),
                    context?.Output ?? Console.Out,
                    context?.Error ?? Console.Error);
                commandInstance.SetContext(commandContext);

                return commandInstance.Execute(resolvedCommand, commandContext);
            }

            // Log null check failure
            Console.Error.WriteLine("Failed to create command instance.");
            return (int)ErrorCode.ExecutionFailure;
        }

        // Command not found, check path for external commands
        string pathVar = GetVariable("path") ?? "/bin:/usr/bin";
        var paths = pathVar.Split(':', StringSplitOptions.RemoveEmptyEntries);

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
            Console.Error.WriteLine($"Failed to find the command `{command}` in path");
            return (int)ErrorCode.CommandNotFound;
        }

        try
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = executablePath;
            process.StartInfo.UseShellExecute = false;

            foreach (var argument in resolvedCommand.Skip(1))
                process.StartInfo.ArgumentList.Add(argument);

            bool shouldRedirectStreams = forceCaptureStreams || context is not null || Console.IsOutputRedirected || Console.IsErrorRedirected || standardInput is not null;
            process.StartInfo.RedirectStandardOutput = shouldRedirectStreams;
            process.StartInfo.RedirectStandardError = shouldRedirectStreams;
            process.StartInfo.RedirectStandardInput = standardInput is not null;

            process.StartInfo.CreateNoWindow = shouldRedirectStreams;

            process.Start();

            if (standardInput is not null)
            {
                process.StandardInput.Write(standardInput);
                process.StandardInput.Close();
            }

            if (shouldRedirectStreams)
            {
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                    (context?.Output ?? Console.Out).Write(output);

                if (!string.IsNullOrEmpty(error))
                    (context?.Error ?? Console.Error).Write(error);
            }
            else
            {
                process.WaitForExit();
            }

            return process.ExitCode;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[Error] Failed to run external command: {ex.Message}");
            return (int)ErrorCode.CommandNotFound;
        }
    }

    /// <summary>
    /// Execute the current command buffer.
    /// </summary>
    public int CommandSwitch() => ExecuteCommandArray(commandArguments);

    /// <summary>
    /// Tick - Read input and execute input
    /// </summary>
    public void Tick()
    {
        var input = ReadScriptInput();

        // Execute the command
        ExecuteAndCapture(input);
    }

    private string ReadScriptInput()
    {
        var lines = new List<string>();
        string firstLine = Prompt();
        lines.Add(firstLine);

        while (ScriptEngine.NeedsMoreInput(string.Join("\n", lines)))
        {
            int blockDepth = ScriptEngine.GetBlockDepth(string.Join("\n", lines));
            string continuationPrompt = new string('\t', blockDepth) + "> ";
            lines.Add(ReadInline(continuationPrompt));
        }

        return string.Join("\n", lines);
    }

    /// <summary>
    /// Is the command running interactive
    /// </summary>
    /// <returns>True if command is interactive, else false</returns>
    private bool IsCommandInteractive() => !Console.IsInputRedirected && !Console.IsOutputRedirected;

    /// <summary>
    /// Get the reference to the AzzyShell
    /// </summary>
    /// <returns>Reference to AzzyShell</returns>
    public static Azzy GetInstance() => instance!;

    /// <summary>
    /// Displays prompt, capturing the prompt once enter his hit
    /// </summary>
    /// <returns>Raw prompt input</returns>
    private string Prompt()
    {
        string promptStr = GetVariable("prompt") ?? "~";

        // Add status code if non-zero
        var status = GetVariable("status");
        if (status != "0")
            promptStr = $"({status}) {promptStr}";

        string input = ReadInline(promptStr + " ");

        // Append to history
        AppendHistoryEntry(input);

        return input;
    }

    private static string ReadInline(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

}
