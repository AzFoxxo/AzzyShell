namespace App;
using Heroes;
using Commands;
using System.Text.RegularExpressions;

// A test hero
public class AzzyShell : Hero
{
    private const string version = "0.1.2";
    private const string shell = "AzzyShell";
    private const string author = "Az Foxxo";
    private const string description = "A simple shell to test the Heroes framework.";
    private const string prompt = "Azzy~";

    int returnedCode = 0;
    private static AzzyShell? instance;

    public List<Variables> variables = new();

    private string[] args = new string[0];

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
        // Run the welcome command
        returnedCode = new Welcome().Execute(new string[] { "welcome" });

        // Run the logo command
        returnedCode = new Logo().Execute(new string[] { "logo" });
    }

    public override void OnUpdate()
    {
        // Get the input
        string input = GetCurrentLine();

        // Split the input into several commands if there are multiple commands or a new line
        string[] commands = input.Split(new string[] { "&&", "\n" }, StringSplitOptions.RemoveEmptyEntries);

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

        return Read(variables.Find(x => x.name == "prompt").value + " ", heroesColour);
    }

    public static AzzyShell GetInstance() => instance!;
    public int CommandSwitch()
    {
        // Check if the first argument is a command
        switch (args[0])
        {
            case "help":
                return new Help().Execute(args);
            case "quit":
                return new Quit().Execute(args);
            case "pwd":
                return new PWD().Execute(args);
            case "cd":
                return new CD().Execute(args);
            case "ls":
                return new LS().Execute(args);
            case "clear":
                return new Clear().Execute(args);
            case "touch":
                return new Touch().Execute(args);
            case "remove":
                return new Remove().Execute(args);
            case "mkdir":
                return new MKDir().Execute(args);
            case "log":
                return new Log().Execute(args);
            case "cat":
                return new Cat().Execute(args);
            case "fizzbuzz":
                return new FizzBuzz().Execute(args);
            case "var":
                return new Var().Execute(args);
            case "set":
                return new Set().Execute(args);
            case "vars":
                return new Vars().Execute(args);
            case "logo":
                return new Logo().Execute(args);
            case "hacker":
                return new Hacker().Execute(args);

            // Azzy internal commands
            case "azzy_welcome":
                return new Welcome().Execute(args);

            // If the command is not found, return 1 and print an error message
            default:

                // Launch the command in system shell
                returnedCode = new External().Execute(args);

                // System not external command
                if (returnedCode == 0)
                {
                    Print($"Failed to find the command `{args[0]}`", Colours.Red);
                    return 1;
                } else return returnedCode;
        }
    }
}
