namespace Flex.Shared.SeedWork
{
    public abstract class PagingRequestParameters
    {
        // Page Index
        private int _pageIndex = 1;
        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value < 1 ? 1 : value;
        }

        // Page Size
        public int? PageSize { get; set; }

        private string? _orderBy;
        public string? OrderBy
        {
            get => _orderBy;
            set => _orderBy = value?.Trim().ToUpper();
        }

        private string? _sortBy;
        public string? SortBy
        {
            get => _sortBy;
            set => _sortBy = value?.Trim().ToUpper();
        }

        protected virtual Dictionary<string, string> OrderByMappings => new();
    }
}
