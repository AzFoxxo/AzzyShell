/*
*   New AzzyShell entry point.
*   This replaces Heroes framework
*/

namespace AzzyShell;

public static class Program
{
    private static readonly Azzy azzy = new();
    public static bool Running { get; set; } = true;
    public static void Main(string[] args)
    {
        while (Running)
        {
            // AzzyShell tick
            azzy.Tick();
        }
    }
}