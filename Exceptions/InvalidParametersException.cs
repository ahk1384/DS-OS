using System.Threading.Channels;

namespace DS_OS.Exceptions;

public class InvalidParametersException : Exception
{
    public InvalidParametersException()
        : base("The Wrong Parameters Typed.")
    {
    }

    public InvalidParametersException(string message)
        : base(message)
    {
    }

    public InvalidParametersException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}