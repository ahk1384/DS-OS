namespace DS_OS.Exceptions
{
    /// <summary>
    /// Exception thrown when a file required by a process is not found.
    /// </summary>
    internal class FileNotFoundException : Exception
    {
        public FileNotFoundException() 
            : base("The required file was not found.")
        {
        }

        public FileNotFoundException(string message) 
            : base(message)
        {
        }

        public FileNotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
