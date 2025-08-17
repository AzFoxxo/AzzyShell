namespace App;
using Heroes;
using Commands;
using System.Text.RegularExpressions;
using System.Reflection;

// A test hero
[AutoInitialise]
public class AzzyShell : Hero
{
    private const string version = "2.0.0";
    private const string shell = "Azzy";
    private const string author = "Az Foxxo";
    private const string description = "A simple shell written in C# and Heroes framework";
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

    // Print the welcome message
    public override void OnStart()
    {
        // Start-up commans
        returnedCode = new Clear().Execute(["clear"]);
        returnedCode = new Welcome().Execute(["welcome"]);
        returnedCode = new Logo().Execute(["logo"]);

        // Check if the history file exists in the home directory
        if (!File.Exists(historyFile))
        {
            // Create the history file
            File.Create(historyFile);
        }
    }

    public override void OnUpdate()
    {
        // Get the input
        string input = GetCurrentLine();

        // Split the input into several commands if there are multiple commands or a new line
        var commands = input.Split(["&&", "\n"], StringSplitOptions.RemoveEmptyEntries);

        // Loop through the commands
        foreach (string command in commands)
        {
            // Delete any spaces at the start and end of the command
            string trimmedCommand = command.Trim();

            #region @Replaces@
            // Replace @DOUBLE@ with a UUID
            string doubleQuote = Guid.NewGuid().ToString();
            trimmedCommand = Regex.Replace(trimmedCommand, "@DOUBLE@", doubleQuote);

            // Replace @AND@ with with a UUID
            string and = Guid.NewGuid().ToString();
            trimmedCommand = Regex.Replace(trimmedCommand, "@AND@", and);

            // Replace @NEWLINE@ with a UUID
            string newline = Guid.NewGuid().ToString();
            trimmedCommand = Regex.Replace(trimmedCommand, "@NEWLINE@", newline);

            // Replace @AT@ with a UUID
            string at = Guid.NewGuid().ToString();
            trimmedCommand = Regex.Replace(trimmedCommand, "@AT@", at);

            // Replace @TAB@ with a UUID
            string tab = Guid.NewGuid().ToString();
            trimmedCommand = Regex.Replace(trimmedCommand, "@TAB@", tab);
            #endregion

            // Split the command into arguments
            args = Regex.Matches(trimmedCommand, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray();

            // Remove all quotes from the arguments
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Replace("\"", "");
            }

            // Replace the UUIDs with the original characters
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Replace(doubleQuote, "\"");
                args[i] = args[i].Replace(and, "&&");
                args[i] = args[i].Replace(newline, "\n");
                args[i] = args[i].Replace(at, "@");
                args[i] = args[i].Replace(tab, "\t");
            }

            // Check if any arguments were given
            if (args.Length < 1) return;

            // Check not whitespace
            if (args[0] == "") return;

            // Find "returnedCode" variable and update it
            Variables[] variablesArray = variables.ToArray();
            for (int i = 0; i < variablesArray.Length; i++)
            {
                if (variablesArray[i].name == "returnedCode")
                {
                    variablesArray[i].value = returnedCode.ToString();
                    variables[i] = variablesArray[i];
                    break;
                }
            }

            // Find the command
            returnedCode = CommandSwitch();

            // Update the "returnedCode" variable
            for (int i = 0; i < variablesArray.Length; i++)
            {
                if (variablesArray[i].name == "returnedCode")
                {
                    variablesArray[i].value = returnedCode.ToString();
                    variables[i] = variablesArray[i];
                    break;
                }
            }
        }
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
        string input = Read(variables.Find(x => x.name == "prompt").value + " ", heroesColour);

        // Add the line to the history file
        File.AppendAllText(historyFile, input + "\n");

        return input;
    }

    public static AzzyShell GetInstance() => instance!;

    public int CommandSwitch()
    {
        // Argument translation
        var resolvedCommand = Command.VariableTranslation(args);

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
