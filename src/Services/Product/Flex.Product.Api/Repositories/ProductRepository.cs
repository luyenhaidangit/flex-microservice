using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Product.Api.Entities;
using Flex.Product.Api.Persistence;
using Flex.Product.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Product.Api.Repositories
{
    public class ProductRepository : RepositoryBase<CatalogProduct, long, ProductContext>, IProductRepository
    {
        public ProductRepository(ProductContext dbContext, IUnitOfWork<ProductContext> unitOfWork) : base(dbContext, unitOfWork)
        {
        }

        public async Task<IEnumerable<CatalogProduct>> GetProductsAsync() => await FindAll().ToListAsync();

        public Task<CatalogProduct> GetProductAsync(long id) => GetByIdAsync(id);

        public Task<CatalogProduct> GetProductByNoAsync(string productNo) =>
            FindByCondition(x => x.No.Equals(productNo)).SingleOrDefaultAsync();

        public Task CreateProductAsync(CatalogProduct product) => CreateAsync(product);

        public Task UpdateProductAsync(CatalogProduct product) => UpdateAsync(product);

        public async Task DeleteProductAsync(long id)
        {
            var product = await GetProductAsync(id);
            if (product != null) await DeleteAsync(product);
        }
    }
}
