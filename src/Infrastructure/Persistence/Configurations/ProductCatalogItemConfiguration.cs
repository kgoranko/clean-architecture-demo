using Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class ProductCatalogItemConfiguration : IEntityTypeConfiguration<ProductCatalogItem>
{
    public void Configure(EntityTypeBuilder<ProductCatalogItem> builder)
    {
        builder.ToTable("ProductCatalog", "Demo");

        builder.HasKey(product => product.ProductName);

        builder.Property(product => product.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(product => product.StockQuantity)
            .IsRequired();

        builder.Property(product => product.IsActive)
            .IsRequired();
    }
}
