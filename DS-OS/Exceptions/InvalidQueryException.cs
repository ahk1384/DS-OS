namespace DS_OS.Exceptions;

public class InvalidQueryException : Exception
{
    public InvalidQueryException()
        : base("The specified parent process is invalid.")
    {
    }

    public InvalidQueryException(string message)
        : base(message)
    {
    }

    public InvalidQueryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
};