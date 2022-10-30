namespace App;
using Heroes;
using Commands;

// A test hero
public class AzzyShell : Hero
{
    string version = "0.0.1";
    string shell = "AzzyShell";
    string author = "Az Foxxo";
    string description = "A shell to test the Heroes framework";

    string prompt = "~";

    int ret = 0;

    public override void OnUpdate()
    {
        // Get the input
        string input = GetCurrentLine();

        // Split the input into an array of strings
        string[] args = input.Split(' ');

        // Check if any arguments were given
        if (args.Length < 1) return;

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

    private string GetCurrentLine() => Read(prompt);
}
