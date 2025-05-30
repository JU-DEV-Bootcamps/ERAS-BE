namespace Eras.Application.Models.Response.Common
{
    public class QueryManyResponse<T>: BaseResponse
    {
        public IEnumerable<T> Entities { get; set; }

        public QueryManyResponse(IEnumerable<T> Entities)
        {
            this.Entities = Entities;
        }

        public QueryManyResponse(IEnumerable<T> Entities, string Message, bool Success) : base(Message, Success)
        {
            this.Entities = Entities;
        }
    }
}
