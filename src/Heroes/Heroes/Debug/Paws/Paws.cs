using System;
using System.IO;

namespace Heroes.Debug.Paws;

public static class PawsLogger
{
    private static Levels _DefaultLevel;
    private static bool _logToFile;
    private static bool _logTime;
    private static string? _logPath;
    private static string? _logName;

    private static bool _initialized = false;

    /// <summary> Setup the Paws logger. </summary>
    /// <param name="logToFile"> Log to a file </param>
    /// <param name="logToConsole"> Log to the console </param>
    /// <param name="logTime"> Include the time in the log </param>
    /// <param name="logPath"> Destination for log files (not file) </param>
    /// <param name="DefaultLevel"> Default log level </param>
    public static void Setup(bool logToFile = false, bool logTime = false, string logPath = "log/", Levels DefaultLevel = Levels.Info)
    {
        // Set the values for the logger
        _logToFile = logToFile;
        _logTime = logTime;
        _logPath = logPath;
        _DefaultLevel = DefaultLevel;

        // Generate the log file name based on the current date and time (to milliseconds)
        _logName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + ".log";

        // Set initialized to true
        _initialized = true;
    }

    #region Specific Level Log Methods
    /// <summary> Log an error message to the console. </summary>
    /// <param name="message"> The message to log. </param>
    /// <returns> The message that was logged represented as a string. </returns>
    public static string Error<T>(T value) => Log(value, Levels.Error);
    /// <summary> Log a warning message to the console. </summary>
    /// <param name="message"> The message to log. </param>
    /// <returns> The message that was logged represented as a string. </returns>
    public static string Warn<T>(T value) => Log(value, Levels.Warn);
    /// <summary> Log an info message to the console. </summary>
    /// <param name="message"> The message to log. </param>
    /// <returns> The message that was logged represented as a string. </returns>
    public static string Info<T>(T value) => Log(value, Levels.Info);
    /// <summary> Log a message to the console. </summary>
    /// <param name="message"> The message to log. </param>
    /// <returns> The message that was logged represented as a string. </returns>
    public static string Debug<T>(T value) => Log(value, Levels.Debug);
    /// <summary> Log a message to the console (using default message priority). </summary>
    /// <param name="message"> The message to log. </param>
    /// <returns> The message that was logged represented as a string. </returns>
    public static string Log<T>(T value) => Log(value, _DefaultLevel);
    #endregion

    /// <summary> Log a message to the console. </summary>
    /// <param name"paws"> The Paws instance </param>
    /// <param name="message"> The message to log. </param>
    /// <param name="level"> The level of the message. </param>
    /// <returns> The message that was logged represented as a string. </returns>
    public static string Log<T>(T value, Levels level = Levels.Info)
    {
        // Check if Instance is not null
        if (!_initialized) throw new Exception("Paws is not initialized. Please call Paws.Setup() before using Paws.");

        // Get the current colour
        var color = Console.ForegroundColor;

        // Set the prefix based on the level
        var prefix = level switch
        {
            Levels.Error => "[ERROR]",
            Levels.Warn => "[WARN]",
            Levels.Info => "[INFO]",
            Levels.Debug => "[DEBUG]",
            _ => "[INFO]"
        };

        // Set the colour based on the level
        Console.ForegroundColor = level switch
        {
            Levels.Error => ConsoleColor.Red,
            Levels.Warn => ConsoleColor.Yellow,
            Levels.Info => ConsoleColor.White,
            Levels.Debug => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };

        // Message to log
        var message = $"{prefix} {(_logTime ? DateTime.Now.ToString("HH:mm:ss.fff") : "")} {value}";

        // Log the message
        Console.WriteLine(message);

        // Reset the colour
        Console.ForegroundColor = color;

        // Log the message to a file
        if (_logToFile)
        {
            // Check the path exists
            if (!Directory.Exists(_logPath))
            {
                // Create the directory
                Directory.CreateDirectory(_logPath!);
            }

            // Create the file if it doesn't exist
            if (!File.Exists(_logPath + _logName))
            {
                // Store the previous path
                var previousPath = Directory.GetCurrentDirectory();

                // Change directory to the log path
                Directory.SetCurrentDirectory(_logPath!);

                // Create the file
                File.Create(_logName!).Dispose();

                // Change directory back to the original
                Directory.SetCurrentDirectory(previousPath);
            }

            // Write the message to the file
            using (var writer = new StreamWriter(_logPath + _logName, true))
            {
                writer.WriteLine(message);
            }
        }

        // Reset the colour
        return message;
    }
}

