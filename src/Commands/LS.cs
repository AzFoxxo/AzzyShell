namespace AzzyShell.Commands;

class Ls : Command
{
    public override int Execute(string[] args)
    {
        if (!IsArgumentLengthValid(args, 1, allowGreaterThanLength: true))
            return (int)ErrorCode.InvalidArguments;

        var paths = args.Skip(1).ToArray();

        if (paths.Length == 0)
        {
            paths = [Directory.GetCurrentDirectory()];
        }

        foreach (var path in paths)
        {
            if (!Directory.Exists(path))
            {
                PrintLine($"Directory not found: {path}", Colours.Red);
                return (int)ErrorCode.DirectoryNotFound;
            }

            var directory = new DirectoryInfo(path);

            foreach (var dir in directory.GetDirectories())
            {
                Print($"{dir.Name}  ", Colours.Blue);
            }

            foreach (var file in directory.GetFiles())
            {
                Print($"{file.Name}  ", Colours.Green);
            }

            PrintLine("");

            if (paths.Length > 1)
            {
                PrintLine("");
            }
        }

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        "List files and directories: [path1] [path2] ...";
}