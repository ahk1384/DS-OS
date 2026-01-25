namespace DS_OS.Exceptions;


public class InvalidProcessFormat : Exception
{
    public InvalidProcessFormat()
        : base("The specified process format is invalid.")
    {
    }

    public InvalidProcessFormat(string message)
        : base(message)
    {
    }

    public InvalidProcessFormat(string message, Exception innerException)
        : base(message, innerException)
    {
    }

}
