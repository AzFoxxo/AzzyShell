namespace Heroes.Internal;

public static class Program
{
    // Main loop
    public static GameLoop gameLoop = new();

    // Entry point
    public static void Main(string[] args) => gameLoop.Enter();
}
