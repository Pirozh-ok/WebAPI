namespace Habr.Common.DTOs.ResponceDTOs
{
    public class ClientErrorResponse
    {
        public ClientErrorResponse(string errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
