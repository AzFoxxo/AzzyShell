using Heroes.Debug.Paws;

namespace Heroes.Internal;

public static class Program
{
    // Main loop
    public static GameLoop gameLoop = new();

    // Entry point
    public static void Main(string[] args) {
        // Initialise the Paws logger
        PawsLogger.Setup(true, true, "logs/", Levels.Debug);
        PawsLogger.Info("Paws logger initialised");
        
        // Run the game loop
        gameLoop.Enter();
    }
}
