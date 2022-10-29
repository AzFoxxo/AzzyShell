namespace Heroes;
using Heroes.Internal;

// Application class
public static class App
{
    ///<summary>Quit the application.</summary>
    public static void End() => GameLoop.running = false;

    ///<summary>Restart the application.</summary>
    public static void Restart() => Program.gameLoop.Restart();

}
