namespace Habr.Common.Exceptions
{
    public class AuthorizationException : Exception
    {
        public int ErrorCode { get; set; } = 403;
        public AuthorizationException(string message)
            : base(message)
        { }
    }
}
