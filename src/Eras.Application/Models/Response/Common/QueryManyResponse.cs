namespace Eras.Application.Models.Response.Common
{
    public class QueryManyResponse<T>: BaseResponse
    {
        public IEnumerable<T> Entities { get; set; }

        public QueryManyResponse(IEnumerable<T> entities)
        {
            Entities = entities;
        }

        public QueryManyResponse(IEnumerable<T> entities, string message, bool success) : base(message, success)
        {
            Entities = entities;
        }
    }
}
