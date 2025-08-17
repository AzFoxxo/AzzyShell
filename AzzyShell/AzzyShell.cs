namespace App;
using Heroes;
using Commands;
using System.Text.RegularExpressions;
using System.Reflection;

// A test hero
[AutoInitialise]
public class AzzyShell : Hero
{
    private const string version = "2.1.0";
    private const string shell = "Azzy";
    private const string author = "Az Foxxo";
    private const string description = "A simple shell written in C# and Heroes framework.";
    private const string prompt = "$";

    int returnedCode = 0;
    private static AzzyShell? instance;

    public List<Variables> variables = [];

    private string[] args = [];

    public string historyFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".history.azzy");

    // Add shell variables and store a reference to the shell
    public override void OnEarlyStart()
    {
        variables.Add(new Variables("version", version, "String"));
        variables.Add(new Variables("shell", shell, "String"));
        variables.Add(new Variables("author", author, "String"));
        variables.Add(new Variables("description", description, "String"));
        variables.Add(new Variables("prompt", prompt, "String"));
        variables.Add(new Variables("returnedCode", returnedCode.ToString(), "Int"));
        variables.Add(new Variables("colour", "Cyan", "String"));

        instance = this;
    }

    // Load configuration
    public override void OnStart()
    {
        string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string initFilePath = Path.Combine(homeDirectory, ".azzyshell_init.ass");

        // Check if the config file exists
        if (File.Exists(initFilePath))
        {
            // If the config file exists, run it using the "run" command
            returnedCode = new Run().Execute(new string[] { "run", initFilePath });

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
            returnedCode = new Clear().Execute(new string[] { "clear" });
            returnedCode = new Welcome().Execute(new string[] { "welcome" });
            returnedCode = new Logo().Execute(new string[] { "logo" });
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
        var commands = input.Split(new[] { "&&", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        int lastReturnedCode = 0;

        foreach (string command in commands)
        {
            string trimmedCommand = command.Trim();

            // Skip the line if it starts with # (comment)
            if (trimmedCommand.StartsWith("#"))
                continue;

            // Remove inline comments (anything after #)
            int commentIndex = trimmedCommand.IndexOf('#');
            if (commentIndex >= 0)
            {
                trimmedCommand = trimmedCommand.Substring(0, commentIndex).Trim();
            }

            #region @Replaces@
            string doubleQuote = Guid.NewGuid().ToString();
            trimmedCommand = Regex.Replace(trimmedCommand, "@DOUBLE@", doubleQuote);

            string and = Guid.NewGuid().ToString();
            trimmedCommand = Regex.Replace(trimmedCommand, "@AND@", and);

            string newline = Guid.NewGuid().ToString();
            trimmedCommand = Regex.Replace(trimmedCommand, "@NEWLINE@", newline);

            string at = Guid.NewGuid().ToString();
            trimmedCommand = Regex.Replace(trimmedCommand, "@AT@", at);

            string tab = Guid.NewGuid().ToString();
            trimmedCommand = Regex.Replace(trimmedCommand, "@TAB@", tab);

            string hash = Guid.NewGuid().ToString();
            trimmedCommand = Regex.Replace(trimmedCommand, "@HASH@", tab);
            #endregion

            // Split into args
            var args = Regex.Matches(trimmedCommand, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray();

            // Clean args
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Replace("\"", "");
                args[i] = args[i].Replace(doubleQuote, "\"");
                args[i] = args[i].Replace(and, "&&");
                args[i] = args[i].Replace(newline, "\n");
                args[i] = args[i].Replace(at, "@");
                args[i] = args[i].Replace(tab, "\t");
                args[i] = args[i].Replace(hash, "#");
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


    private void UpdateReturnedCodeVariable(int code)
    {
        var variablesArray = variables.ToArray();
        for (int i = 0; i < variablesArray.Length; i++)
        {
            if (variablesArray[i].name == "returnedCode")
            {
                variablesArray[i].value = code.ToString();
                variables[i] = variablesArray[i];
                break;
            }
        }
    }

    public override void OnUpdate()
    {
        // Get the input
        var input = GetCurrentLine();

        // Execute the command
        ExecuteCommand(input);
    }

    private string GetCurrentLine()
    {
        // Convert string colour to heroes colour
        string colour = variables.Find(x => x.name == "colour").value;

        // Try and parse the colour to a heroes colours
        if (!Enum.TryParse(colour, true, out Colours heroesColour))
        {
            // If it fails, set the colour to white
            heroesColour = Colours.Red;
        }

        // Read the current line
        string input = ReadInline(variables.Find(x => x.name == "prompt").value + " ", heroesColour);

        // Add the line to the history file
        File.AppendAllText(historyFile, input + "\n");

        return input;
    }

    public static AzzyShell GetInstance() => instance!;

    public int CommandSwitch()
    {
        // Argument translation
        var resolvedCommand = args;
        try
        {
            resolvedCommand = Command.VariableResolution(args);
        }
        catch (Exception ex)
        {
            PrintLine($"[Error] {ex.Message}", Colours.Red);
            return 3;
        }


        if (resolvedCommand.Length == 0)
            return 1;

        var commandKey = resolvedCommand[0];

        // Find command type by name (case-insensitive) among all Command subclasses
        var commandType = Assembly.GetExecutingAssembly()
            .GetTypes()
            .FirstOrDefault(t =>
                typeof(Command).IsAssignableFrom(t) &&
                !t.IsAbstract &&
                string.Equals(t.Name, commandKey, StringComparison.OrdinalIgnoreCase)
            );

        if (commandType != null)
        {
            var commandInstance = (Command)Activator.CreateInstance(commandType);
            return commandInstance.Execute(resolvedCommand);
        }

        // Fallback to external system shell command
        int returnedCode = new External().Execute(args);

        if (returnedCode == 0)
        {
            PrintLine($"Failed to find the command `{args[0]}`", Colours.Red);
            return 1;
        }

        return returnedCode;
    }

}
