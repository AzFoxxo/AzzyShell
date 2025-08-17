namespace AzzyShell;

/// <summary>
/// Create a new shell variable
/// </summary>
/// <param name="name">Variable name in azzyshell</param>
/// <param name="value">Variable value</param>
/// <param name="type">Variable type</param>
public class ShellVariable(string name, string value, string type)
{
    public string Name { get; set; } = name;
    public string Value { get; set; } = value;
    public string Type { get; set; } = type;
}
