namespace Habr.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public int ErrorCode { get; set; } = 404;
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
