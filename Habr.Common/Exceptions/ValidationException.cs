namespace Habr.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public int ErrorCode { get; set; } = 400;
        public ValidationException(string message) 
            : base(message)
        {
        }
    }
}
