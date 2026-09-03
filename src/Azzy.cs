namespace AzzyShell;

using System.Reflection;
using AzzyShell.Commands;

partial class Azzy
{
    // Singleton
    private static Azzy? instance;

    // Constants
    private const string version = "3.1.1";
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
            resolvedCommand = ResolveScriptTokens(resolvedCommand)
                .Select(ScriptText.UnwrapLiteral)
                .ToArray();
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
    private bool IsCommandInteractive() => !Console.IsInputRedirected;

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

    private string ReadInline(string prompt)
    {
        if (!IsCommandInteractive())
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        Console.Write(prompt);
        var input = new List<char>();
        int cursor = 0;
        int historyIndex = -1;
        string draftInput = string.Empty;
        bool completionDisplayed = false;
        List<string>? completionOptions = null;
        int completionSelection = -1;
        int completionTokenStart = -1;

        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                if (completionOptions is not null && completionSelection >= 0)
                {
                    input.RemoveRange(completionTokenStart, cursor - completionTokenStart);
                    input.InsertRange(completionTokenStart, completionOptions[completionSelection]);
                    cursor = completionTokenStart + completionOptions[completionSelection].Length;
                    completionOptions = null;
                    completionSelection = -1;
                    completionTokenStart = -1;
                    completionDisplayed = false;
                    RedrawInput(prompt, input, cursor);
                    continue;
                }

                Console.WriteLine();
                return new string(input.ToArray());
            }

            if (key.Key == ConsoleKey.Tab)
            {
                if (completionOptions is not null)
                {
                    completionSelection = (completionSelection + 1) % completionOptions.Count;
                    RenderCompletionMenu(prompt, input, cursor, completionOptions, completionSelection, menuExists: true);
                    continue;
                }

                CompleteInput(prompt, input, ref cursor, ref completionDisplayed,
                    ref completionOptions, ref completionSelection, ref completionTokenStart);
                continue;
            }

            if (completionOptions is not null && key.Key is ConsoleKey.UpArrow or ConsoleKey.DownArrow)
            {
                completionSelection = key.Key == ConsoleKey.UpArrow
                    ? (completionSelection - 1 + completionOptions.Count) % completionOptions.Count
                    : (completionSelection + 1) % completionOptions.Count;
                RenderCompletionMenu(prompt, input, cursor, completionOptions, completionSelection, menuExists: true);
                continue;
            }

            completionDisplayed = false;
            completionOptions = null;
            completionSelection = -1;
            completionTokenStart = -1;

            if (key.Key == ConsoleKey.UpArrow)
            {
                if (historyEntries.Count > 0)
                {
                    if (historyIndex == -1)
                        draftInput = new string(input.ToArray());

                    historyIndex = Math.Min(historyIndex + 1, historyEntries.Count - 1);
                    input = [.. historyEntries[historyEntries.Count - historyIndex - 1]];
                    cursor = input.Count;
                    RedrawInput(prompt, input, cursor);
                }

                continue;
            }

            if (key.Key == ConsoleKey.DownArrow)
            {
                if (historyIndex >= 0)
                {
                    historyIndex--;
                    input = historyIndex == -1
                        ? [.. draftInput]
                        : [.. historyEntries[historyEntries.Count - historyIndex - 1]];
                    cursor = input.Count;
                    RedrawInput(prompt, input, cursor);
                }

                continue;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                historyIndex = -1;
                if (cursor > 0)
                {
                    input.RemoveAt(--cursor);
                    RedrawInput(prompt, input, cursor);
                }

                continue;
            }

            if (key.Key == ConsoleKey.Delete)
            {
                historyIndex = -1;
                if (cursor < input.Count)
                {
                    input.RemoveAt(cursor);
                    RedrawInput(prompt, input, cursor);
                }

                continue;
            }

            if (key.Key == ConsoleKey.LeftArrow)
            {
                if (cursor > 0)
                {
                    cursor--;
                    Console.Write("\u001b[1D");
                }

                continue;
            }

            if (key.Key == ConsoleKey.RightArrow)
            {
                if (cursor < input.Count)
                {
                    Console.Write(input[cursor++]);
                }

                continue;
            }

            if (key.Key == ConsoleKey.Home)
            {
                cursor = 0;
                RedrawInput(prompt, input, cursor);
                continue;
            }

            if (key.Key == ConsoleKey.End)
            {
                cursor = input.Count;
                RedrawInput(prompt, input, cursor);
                continue;
            }

            if (!char.IsControl(key.KeyChar))
            {
                historyIndex = -1;
                input.Insert(cursor++, key.KeyChar);
                RedrawInput(prompt, input, cursor);
            }
        }
    }

    private void CompleteInput(
        string prompt,
        List<char> input,
        ref int cursor,
        ref bool completionDisplayed,
        ref List<string>? completionOptions,
        ref int completionSelection,
        ref int completionTokenStart)
    {
        try
        {
            string currentInput = new(input.ToArray());
            int tokenStart = cursor;
            while (tokenStart > 0 && !char.IsWhiteSpace(input[tokenStart - 1]) &&
                   !IsCommandSeparator(input[tokenStart - 1]))
                tokenStart--;

            string token = currentInput[tokenStart..cursor];
            bool completingCommand = IsCommandPosition(currentInput, tokenStart);
            var candidates = completingCommand
                ? GetCommandCompletions(token)
                : GetPathCompletions(token);

            if (candidates.Count == 0)
                return;

            string commonPrefix = GetCommonPrefix(candidates);
            bool showOptions = completionDisplayed || commonPrefix == token;
            if (candidates.Count == 1 || (!completionDisplayed && commonPrefix.Length > token.Length))
            {
                input.RemoveRange(tokenStart, cursor - tokenStart);
                input.InsertRange(tokenStart, candidates.Count == 1 ? candidates[0] : commonPrefix);
                cursor = tokenStart + (candidates.Count == 1 ? candidates[0].Length : commonPrefix.Length);
                completionDisplayed = candidates.Count > 1 && commonPrefix.Length > token.Length;
                RedrawInput(prompt, input, cursor);
            }

            if (candidates.Count > 1 && showOptions)
            {
                completionOptions = candidates;
                completionSelection = 0;
                completionTokenStart = tokenStart;
                RenderCompletionMenu(prompt, input, cursor, candidates, completionSelection, menuExists: false);
                completionDisplayed = true;
            }
        }
        catch (Exception)
        {
        }
    }

    private static bool IsCommandPosition(string input, int tokenStart)
    {
        string beforeToken = input[..tokenStart].TrimEnd();
        return beforeToken.Length == 0 ||
               beforeToken.EndsWith('|') ||
               beforeToken.EndsWith("&&", StringComparison.Ordinal) ||
               beforeToken.EndsWith("||", StringComparison.Ordinal) ||
               beforeToken.EndsWith(';');
    }

    private static bool IsCommandSeparator(char character) => character is '|' or '&' or ';';

    private static void RenderCompletionMenu(
        string prompt,
        List<char> input,
        int cursor,
        IReadOnlyList<string> options,
        int selected,
        bool menuExists)
    {
        if (menuExists)
            Console.Write($"\u001b[{options.Count}A");
        else
            Console.WriteLine();

        for (int index = 0; index < options.Count; index++)
        {
            Console.Write("\r\u001b[K");
            Console.Write(index == selected ? "> " : "  ");
            Console.Write(options[index]);
            Console.WriteLine();
        }

        Console.Write("\r\u001b[K");
        Console.Write(prompt);
        Console.Write(new string(input.ToArray()));
        int charactersAfterCursor = input.Count - cursor;
        if (charactersAfterCursor > 0)
            Console.Write($"\u001b[{charactersAfterCursor}D");
    }

    private List<string> GetCommandCompletions(string prefix)
    {
        var commands = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (typeof(Command).IsAssignableFrom(type) && !type.IsAbstract && type.Namespace == "AzzyShell.Commands")
                commands.Add(type.Name.ToLowerInvariant());
        }

        foreach (string alias in Aliases.Keys)
            commands.Add(alias);

        string pathVar = GetVariable("path") ?? "/bin:/usr/bin";
        foreach (string path in pathVar.Split(':', StringSplitOptions.RemoveEmptyEntries))
        {
            try
            {
                foreach (string file in Directory.EnumerateFiles(path))
                {
                    if (!OperatingSystem.IsWindows() &&
                        (File.GetUnixFileMode(file).HasFlag(UnixFileMode.UserExecute) ||
                         File.GetUnixFileMode(file).HasFlag(UnixFileMode.GroupExecute) ||
                         File.GetUnixFileMode(file).HasFlag(UnixFileMode.OtherExecute)))
                    {
                        commands.Add(Path.GetFileName(file));
                    }
                }
            }
            catch (IOException)
            {
                // An unreadable PATH entry should not break input editing.
            }
            catch (UnauthorizedAccessException)
            {
                // An inaccessible PATH entry should not break input editing.
            }
        }

        return commands
            .Where(command => command.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .OrderBy(command => command, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static List<string> GetPathCompletions(string token)
    {
        string expandedToken = token.StartsWith("~/", StringComparison.Ordinal)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), token[2..])
            : token;
        string directory = Path.GetDirectoryName(expandedToken) ?? ".";
        string namePrefix = Path.GetFileName(expandedToken);

        try
        {
            return Directory.EnumerateFileSystemEntries(directory)
                .Where(path => Path.GetFileName(path).StartsWith(namePrefix, StringComparison.OrdinalIgnoreCase))
                .Select(path =>
                {
                    string completedPath = Path.Combine(Path.GetDirectoryName(token) ?? string.Empty, Path.GetFileName(path));
                    return Directory.Exists(path) ? completedPath + Path.DirectorySeparatorChar : completedPath;
                })
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch (IOException)
        {
            return [];
        }
        catch (UnauthorizedAccessException)
        {
            return [];
        }
    }

    private static string GetCommonPrefix(IReadOnlyList<string> candidates)
    {
        string prefix = candidates[0];
        for (int index = 1; index < candidates.Count; index++)
        {
            int length = 0;
            while (length < prefix.Length && length < candidates[index].Length &&
                   char.ToUpperInvariant(prefix[length]) == char.ToUpperInvariant(candidates[index][length]))
            {
                length++;
            }

            prefix = prefix[..length];
        }

        return prefix;
    }

    private static void RedrawInput(string prompt, List<char> input, int cursor)
    {
        string text = new(input.ToArray());
        Console.Write($"\r{prompt}{text}\u001b[K");

        int charactersAfterCursor = text.Length - cursor;
        if (charactersAfterCursor > 0)
            Console.Write($"\u001b[{charactersAfterCursor}D");
    }

}
