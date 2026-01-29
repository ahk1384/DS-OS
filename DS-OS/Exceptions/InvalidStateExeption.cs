namespace DS_OS.Exceptions
{
    /// <summary>
    /// Exception thrown when a file required by a process is not found.
    /// </summary>
    internal class InvalidStateException : Exception
    {
        public InvalidStateException()
            : base("The State Not Math with required.")
        {
        }

        public InvalidStateException(string message)
            : base(message)
        {
        }

        public InvalidStateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}