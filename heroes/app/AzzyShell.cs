namespace App;
using Heroes;
using Commands;

// A test hero
public class AzzyShell : Hero
{
    private const string version = "0.0.1";
    private const string shell = "AzzyShell";
    private const string author = "Az Foxxo";
    private const string description = "A shell to test the Heroes framework";
    private const string prompt = "~";

    int ret = 0;
    public static AzzyShell Instance;

    public List<Variables> variables = new();

    // Add shell variables and store a reference to the shell
    public AzzyShell() {
        variables.Add(new Variables("version", version, "String"));
        variables.Add(new Variables("shell", shell, "String"));
        variables.Add(new Variables("author", author, "String"));
        variables.Add(new Variables("description", description, "String"));
        variables.Add(new Variables("prompt", prompt, "String"));

        Instance = this;
    }

    public override void OnUpdate()
    {
        // Get the input
        string input = GetCurrentLine();

        // Split the input into an array of strings
        string[] args = input.Split(' ');

        // Check if any arguments were given
        if (args.Length < 1) return;

        // Check not whitespace
        if (args[0] == "") return;

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

        // Check if the first argument is a command
        switch (args[0])
        {
            case "help":
                ret = new Help().Execute(args);
                break;

            case "quit":
                ret = new Quit().Execute(args);
                break;

            case "pwd":
                ret = new PWD().Execute(args);
                break;

            case "cd":
                ret = new CD().Execute(args);
                break;

            case "ls":
                ret = new LS().Execute(args);
                break;

            case "clear":
                ret = new Clear().Execute(args);
                break;

            case "touch":
                ret = new Touch().Execute(args);
                break;

            case "remove":
                ret = new Remove().Execute(args);
                break;

            case "mkdir":
                ret = new MKDir().Execute(args);
                break;
            
            case "log":
                ret = new Log().Execute(args);
                break;

            case "cat":
                ret = new Cat().Execute(args);
                break;

            case "fizzbuzz":
                ret = new FizzBuzz().Execute(args);
                break;

            case "var":
                ret = new Var().Execute(args);
                break;

            case "set":
                ret = new Set().Execute(args);
                break;

            case "vars":
                ret = new Vars().Execute(args);
                break;

            default:
                Print("Unknown command: " + args[0]);
                ret = 1;
                break;
        }

        // Check if the command returned an error
        if (ret != 0)
        {
            Print("Command returned error code: " + ret);
        }
    }

    private string GetCurrentLine() => Read(variables.Find(x => x.name == "prompt").value + " ");
}
