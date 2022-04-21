namespace Unico.Admin.Api.Models
{
    public class ErrorResponse
    {
        public ErrorResponse(string errorKey, string errorMessage)
        {
            key = errorKey;
            message = errorMessage;
        }

        public string key { get; set; }
        public string message { get; set; }
    }
}
