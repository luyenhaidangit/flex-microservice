using Flex.AspNetIdentity.Api.Services.Interfaces;

namespace Flex.AspNetIdentity.Api.Services
{
    public class BranchService : IBranchService
    {
        private readonly HttpClient _httpClient;

        public BranchService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SystemApi");
        }

        public async Task<bool> ValidateBranchExistsAsync(long branchId)
        {
            var response = await _httpClient.GetAsync($"/api/Branch/validate/{branchId}");
            return response.IsSuccessStatusCode;
        }
    }
}
