using Eras.Application.Models.Enums;

namespace Eras.Application.Models.Response.Common
{
    public class GetQueryResponse<T> : BaseResponse
    {
        public T Body { get; set; }
        public QueryEnums.QueryResultStatus Status { get; set; } = QueryEnums.QueryResultStatus.Success;

        public GetQueryResponse(T Body)
        {
            this.Body = Body;
        }

        public GetQueryResponse(T Body, string Message, bool Success) : base(Message, Success)
        {
            this.Body = Body;
        }

        public GetQueryResponse(T Body, string Message, bool Success, QueryEnums.QueryResultStatus Status) : base(Message, Success)
        {
            this.Body = Body;
            this.Status = Status;
        }

    }
}
