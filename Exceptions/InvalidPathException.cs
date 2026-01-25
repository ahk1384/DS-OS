namespace DS_OS.Exceptions
{
    /// <summary>
    /// Exception thrown when a file required by a process is not found.
    /// </summary>
    internal class InvalidPathException : Exception
    {
        public InvalidPathException()
            : base("The Required Path Not Found !")
        {
        }

        public InvalidPathException(string message)
            : base(message)
        {
        }

        public InvalidPathException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}