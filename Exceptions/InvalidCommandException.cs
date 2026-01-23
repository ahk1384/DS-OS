namespace DS_OS.Exceptions
{
    internal class InvalidCommandException : Exception
    {
        public InvalidCommandException() 
            : base("The specified command is invalid.")
        {
        }

        public InvalidCommandException(string message) 
            : base(message)
        {
        }

        public InvalidCommandException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
