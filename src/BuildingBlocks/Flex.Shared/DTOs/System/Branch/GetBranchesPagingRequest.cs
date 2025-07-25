﻿using Flex.Shared.SeedWork;

namespace Flex.Shared.DTOs.System.Branch
{
    public class GetBranchesPagingRequest : PagingRequest
    {
        public string? Keyword { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
        };
    }
}
