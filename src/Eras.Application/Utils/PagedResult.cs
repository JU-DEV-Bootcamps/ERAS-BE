namespace Eras.Application.Utils
{
    public class PagedResult<T>
    {
        public int Count { get; init; }

        public IList<T> Items { get; init; } = Array.Empty<T>();

        public PagedResult(int Count, IList<T> Items)
        {
            this.Count = Count;
            this.Items = Items;
        }

        // used by json deserializer
        private PagedResult() { }

        public static PagedResult<T> Empty()
        {
            return new PagedResult<T> { Count = 0, Items = Array.Empty<T>() };
        }
    }
}
