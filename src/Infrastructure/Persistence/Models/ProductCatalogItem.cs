namespace Infrastructure.Persistence.Models;

internal sealed class ProductCatalogItem
{
    public ProductCatalogItem()
    {
    }

    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; }
}
