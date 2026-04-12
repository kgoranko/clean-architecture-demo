using Domain.Orders;
using Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using SharedKernel.DependencyInjection;

namespace Infrastructure.Persistence;

internal sealed class SqlOrderRepository(DemoDbContext dbContext) : IOrderRepository, IScopedService
{
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Orders.SingleOrDefaultAsync(order => order.Id == id, cancellationToken);

    public Task<bool> ProductExistsAsync(string productName, CancellationToken cancellationToken = default)
    {
        string normalizedProductName = NormalizeProductName(productName);

        return dbContext.ProductCatalog.AnyAsync(
            product => product.ProductName == normalizedProductName && product.IsActive,
            cancellationToken);
    }

    public async Task<int> GetStockQuantityAsync(
        string productName,
        CancellationToken cancellationToken = default)
    {
        string normalizedProductName = NormalizeProductName(productName);

        int? stockQuantity = await dbContext.ProductCatalog
            .Where(product => product.ProductName == normalizedProductName && product.IsActive)
            .Select(static product => (int?)product.StockQuantity)
            .SingleOrDefaultAsync(cancellationToken);

        return stockQuantity ?? 0;
    }

    public async Task<decimal?> GetUnitPriceAsync(
        string productName,
        CancellationToken cancellationToken = default)
    {
        string normalizedProductName = NormalizeProductName(productName);

        return await dbContext.ProductCatalog
            .Where(product => product.ProductName == normalizedProductName && product.IsActive)
            .Select(static product => (decimal?)product.Price)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (dbContext.Entry(order).State == EntityState.Detached)
        {
            bool exists = await dbContext.Orders
                .AnyAsync(existingOrder => existingOrder.Id == order.Id, cancellationToken);

            if (exists)
            {
                dbContext.Orders.Update(order);
            }
            else
            {
                dbContext.Orders.Add(order);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string NormalizeProductName(string productName) =>
        string.Join(' ', (productName ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
}
