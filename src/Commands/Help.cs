using System.Reflection;

namespace AzzyShell.Commands;

class Help : Command
{
    public override int Execute(string[] args, CommandContext context)
    {
        if (!IsArgumentLengthValid(args, 1, allowGreaterThanLength: true))
            return (int)ErrorCode.InvalidArguments;

        if (args.Length < 2)
        {
            PrintLine("Commands:");

            var commands = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    typeof(Command).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    t.Namespace == "AzzyShell.Commands")
                .OrderBy(t => t.Name);

            foreach (var command in commands)
            {
                if (Activator.CreateInstance(command) is Command instance)
                {
                    PrintLine($"- {command.Name.ToLower()} - {instance.HelpString()}");
                }
            }
        }
        else
        {
            foreach (var commandName in args.Skip(1))
            {
                var type = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(t =>
                        typeof(Command).IsAssignableFrom(t) &&
                        !t.IsAbstract &&
                        t.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));

                if (type is null)
                {
                    PrintLine($"Command '{commandName}' not found.", Colours.Red);
                    continue;
                }

                if (Activator.CreateInstance(type) is Command command)
                {
                    PrintLine($"{commandName}: {command.HelpString()}");
                }
                else
                {
                    PrintLine($"Failed to load command '{commandName}'.", Colours.Red);
                }
            }
        }

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        "Display help information: `help` or `help <command>`.";
}