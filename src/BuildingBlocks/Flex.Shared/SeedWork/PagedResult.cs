using System.Text.Json.Serialization;

namespace Flex.Shared.SeedWork
{
    public class PagedResult<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public int FirstRowOnPage => TotalItems > 0 ? (PageIndex - 1) * PageSize + 1 : 0;
        public int LastRowOnPage => (int)Math.Min(PageIndex * PageSize, TotalItems);
        public IEnumerable<T> Items { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? OrderBy { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SortBy { get; set; }

        public static PagedResult<T> Create(int pageIndex, int pageSize, int total, IEnumerable<T> items, string? order = default, string? sort = default)
        {
            return new PagedResult<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItems = total,
                OrderBy = order,
                SortBy = sort,
                Items = items
            };
        }
    }
}
