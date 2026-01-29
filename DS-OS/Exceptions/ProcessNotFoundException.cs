namespace DS_OS.Exceptions
{
    
    internal class ProcessNotFoundException : Exception
    {
        public ProcessNotFoundException() 
            : base("The requested process was not found.")
        {
        }

        public ProcessNotFoundException(string message) 
            : base(message)
        {
        }

        public ProcessNotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
