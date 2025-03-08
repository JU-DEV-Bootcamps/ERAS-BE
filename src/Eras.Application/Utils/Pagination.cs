using System.Diagnostics.CodeAnalysis;

namespace Eras.Application.Utils
{
    [ExcludeFromCodeCoverage]
    public class Pagination
    {
        private int _page = 1;
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Max(0, value);
        }

        public int Page
        {
            get => _page;
            set => _page = value + 1;
        }
    }
}
