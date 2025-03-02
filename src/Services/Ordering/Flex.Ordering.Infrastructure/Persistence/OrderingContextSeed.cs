using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Flex.Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        private readonly ILogger _logger;
        private readonly OrderingDbContext _context;

        public OrderContextSeed(ILogger logger, OrderingDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            if (!_context.Orders.Any())
            {
                await _context.Orders.AddRangeAsync(
                    //new Order
                    //{
                    //    UserName = "customer1",
                    //    FirstName = "customer1",
                    //    LastName = "customer",
                    //    EmailAddress = "customer1@local.com",
                    //    ShippingAddress = "Wollongong",
                    //    InvoiceAddress = "Australia",
                    //    TotalPrice = 250
                    //}
                );
            }
        }
    }
}
