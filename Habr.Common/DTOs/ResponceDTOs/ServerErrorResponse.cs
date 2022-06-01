namespace Habr.Common.DTOs.ResponceDTOs
{
    public class ServerErrorResponse
    {
        public ServerErrorResponse(string errorCode, string errorMessage, string stackTrace)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            StackTrace = stackTrace;
        }

        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
    }
}
