using Flex.Contracts.Domains.Interfaces;
using Flex.Product.Api.Entities;
using Flex.Product.Api.Persistence;

namespace Flex.Product.Api.Repositories.Interfaces
{
    public interface IProductRepository : IRepositoryBase<CatalogProduct, long, ProductContext>
    {
        Task<IEnumerable<CatalogProduct>> GetProductsAsync();
        Task<CatalogProduct> GetProductAsync(long id);
        Task<CatalogProduct> GetProductByNoAsync(string productNo);
        Task CreateProductAsync(CatalogProduct product);
        Task UpdateProductAsync(CatalogProduct product);
        Task DeleteProductAsync(long id);
    }
}
