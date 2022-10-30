namespace App;
using Heroes;
using Commands;

// A test hero
public class AzzyShell : Hero
{
    private const string version = "0.1.0";
    private const string shell = "AzzyShell";
    private const string author = "Az Foxxo";
    private const string description = "A simple shell to test the Heroes framework.";
    private const string prompt = "~";

    int returnedCode;
    private static AzzyShell? instance;

    public List<Variables> variables = new();

    private string[] args = new string[0];

    // Add shell variables and store a reference to the shell
    public override void OnEarlyStart() {
        variables.Add(new Variables("version", version, "String"));
        variables.Add(new Variables("shell", shell, "String"));
        variables.Add(new Variables("author", author, "String"));
        variables.Add(new Variables("description", description, "String"));
        variables.Add(new Variables("prompt", prompt, "String"));

        instance = this;
    }

    // Print the welcome message
    public override void OnStart() {
        // Run the welcome command
        returnedCode = new Welcome().Execute(new string[] { "welcome" });
    }

    public override void OnUpdate()
    {
        // Get the input
        string input = GetCurrentLine();

        // Split the input into an array of strings
        args = input.Split(' ');

        // Check if any arguments were given
        if (args.Length < 1) return;

        // Check not whitespace
        if (args[0] == "") return;

        // Translate the variable
        VariableTranslation();
        
        // Find the command
        CommandSwitch();

        // Check if the command returned an error
        if (returnedCode != 0)
        {
            Print("Command returned error code: " + returnedCode);
        }
    }

    private string GetCurrentLine() => Read(variables.Find(x => x.name == "prompt").value + " ");

    public static AzzyShell GetInstance() => instance!;

    public void VariableTranslation()
    {
        // If $VARIABLE_NAME$ is in the input, replace it with the value of the variable from the variables list
        for (int i = 0; i < args.Length; i++) {
            if (args[i].StartsWith("$") && args[i].EndsWith("$")) {
                string varName = args[i].Substring(1, args[i].Length - 2);
                foreach (Variables var in variables) {
                    if (var.name == varName) {
                        args[i] = var.value;
                    }
                }
            }
        }
        // Else if surrounded by ££, replace it with the variable name
        for (int i = 0; i < args.Length; i++) {
            if (args[i].StartsWith("£") && args[i].EndsWith("£")) {
                string varName = args[i].Substring(1, args[i].Length - 2);
                foreach (Variables var in variables) {
                    if (var.name == varName) {
                        args[i] = var.name;
                    }
                }
            }
        }
        // Else if surrounded by #, replace it with the variable type
        for (int i = 0; i < args.Length; i++) {
            if (args[i].StartsWith("#") && args[i].EndsWith("#")) {
                string varName = args[i].Substring(1, args[i].Length - 2);
                foreach (Variables var in variables) {
                    if (var.name == varName) {
                        args[i] = var.type;
                    }
                }
            }
        }
    }

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

            // Azzy internal commands
            case "azzy_welcome":
                return new Welcome().Execute(args);

            // If the command is not found, return 1 and print an error message
            default:
                Print("Unknown command: " + args[0]);
                return 1;
        }
    }
}
