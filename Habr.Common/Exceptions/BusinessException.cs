namespace Habr.Common.Exceptions
{
    public class BusinessException : Exception
    {
        public int ErrorCode { get; set; } = 400;
        public BusinessException(string message)
            : base(message)
        {
        }
    }
}
