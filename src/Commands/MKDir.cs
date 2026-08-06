namespace AzzyShell.Commands;

class Mkdir : Command
{
    public override int Execute(string[] args)
    {
        if (!IsArgumentLengthValid(args, 2, allowGreaterThanLength: true))
            return (int)ErrorCode.InvalidArguments;

        foreach (var directory in args.Skip(1))
        {
            Directory.CreateDirectory(directory);
        }

        return (int)ErrorCode.Success;
    }

    public override string HelpString() =>
        "Create one or more directories: <dir1> <dir2> ...";
}