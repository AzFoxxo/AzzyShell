namespace Heroes;

public class Time
{
    // Get current time in milliseconds
    public static long GetTime()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    private static double _lastTime = GetTime();
    private static double _deltaTime = 0;

    private static double _timeScale = 1;


    /// <summary> Calculate the delta time </summary>
    internal static void CalculateDeltaTime()
    {
        var currentTime = GetTime();
        _deltaTime = ((currentTime - _lastTime) / 1000.0) * _timeScale;
        _lastTime = currentTime;
    }

    /// <summary> Get the delta time </summary>
    /// <returns> Delta time (double) </returns>
    public static double GetDeltaTime()
    {
        return _deltaTime;
    }


    /// <summary> Set timescale </summary>
    /// <param name="timeScale"> Timescale (double) </param>
    public static void SetTimeScale(double timeScale)
    {
        _timeScale = timeScale;
    }
}