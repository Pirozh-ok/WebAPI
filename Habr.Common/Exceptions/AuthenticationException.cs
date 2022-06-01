namespace Habr.Common.Exceptions
{
    public class AuthenticationException : Exception
    {
        public int ErrorCode { get; set; } = 401;
        public AuthenticationException(string message)
            : base(message)
        { }
    }
}
