namespace Eras.Application.Models
{
    public class ListResponse<T>: BaseResponse
    {
        public int Count { get; init; }

        public IList<T> Items { get; init; } = Array.Empty<T>();

        public ListResponse(int count, IList<T> items)
        {
            Count = count;
            Items = items;
        }
        // parameterless constructor
        public ListResponse()
        {
            Count = 0;
            Items = Array.Empty<T>();
        }

        // used by json deserializer
        private ListResponse(int count)
        {
            Count = count;
        }

        public static ListResponse<T> Empty()
        {
            return new ListResponse<T> { Count = 0, Items = Array.Empty<T>() };
        }
    }
}
