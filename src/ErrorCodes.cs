namespace AzzyShell;

/// <summary>
/// Return and error codes for the shell
/// </summary>
public enum ErrorCode
{
    Success = 0,
    InvalidArguments = 1,
    DirectoryNotFound = 2,
    FileNotFound = 3,
    CommandNotFound = 4,
    VariableNotFound = 5,
    CommandExecutionFailed = 6,
    ConsoleClearFailed = 7,
    PathNotFound = 8,
    FileReadError = 9,
    ExecutionFailure = 10,
    UnknownError = 100, // Unknown (generic) error code 
}
