namespace Heroes;
using Heroes.Internal;

// Application class
public static class Application
{
    ///<summary>Quit the application.</summary>
    public static void Quit() => GameLoop.running = false;

    ///<summary>Restart the application.</summary>
    public static void Restart() => Program.gameLoop.Restart();

}
