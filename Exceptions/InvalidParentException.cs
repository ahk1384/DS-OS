namespace DS_OS.Exceptions
{

    internal class InvalidParentException : Exception
    {
        public InvalidParentException() 
            : base("The specified parent process is invalid.")
        {
        }

        public InvalidParentException(string message) 
            : base(message)
        {
        }

        public InvalidParentException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
