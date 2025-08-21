using Flex.Shared.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.SeedWork
{
    public abstract class PagingRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageIndex must be greater than or equal to 1.")]
        public int? PageIndex { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "PageSize must be greater than or equal to 1.")]
        public int? PageSize { get; set; } = 10;

        [ValidateOrderBy]
        public string? OrderBy { get; set; }

        [ValidateSortBy]
        public string? SortBy { get; set; }

        public int PageIndexValue => Math.Max(1, PageIndex ?? 1);
        public int PageSizeValue => Math.Max(1, PageSize ?? 10);

        protected abstract Dictionary<string, string> OrderByMappings { get; }
    }
}
