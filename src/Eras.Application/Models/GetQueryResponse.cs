namespace Eras.Application.Models
{
    public class GetQueryResponse<T> : BaseResponse
    {
        public T Body { get; set; }

        public GetQueryResponse(T body)
        {
            Body = body;
        }

        public GetQueryResponse(T body, string message, bool success) : base(message, success)
        {
            Body = body;
        }

    }
}
