namespace Eras.Application.Models.Response
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> ValidationErrors { get; set; } = new List<string>();

        public BaseResponse() { }
        public BaseResponse(bool Success)
        {
            this.Success = Success;
        }

        public BaseResponse(string Message)
        {
            Success = true;
            this.Message = Message;
        }

        public BaseResponse(string Message, bool Success)
        {
            this.Success = Success;
            this.Message = Message;
        }

    }
}
