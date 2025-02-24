
namespace Eras.Application.Models
{
    public class GetQueryResponse<T> : BaseResponse
    {
        private List<object> jsonRes;
        private string v1;
        private bool v2;

        public T Body { get; set; }

        public GetQueryResponse(var jsonRes, T body)
        {
            Body = body;
        }

        public GetQueryResponse(T body, string message, bool success) : base(message, success)
        {
            Body = body;
        }

        public GetQueryResponse(List<object> jsonRes, string v1, bool v2)
        {
            this.jsonRes = jsonRes;
            this.v1 = v1;
            this.v2 = v2;
        }
    }
}
