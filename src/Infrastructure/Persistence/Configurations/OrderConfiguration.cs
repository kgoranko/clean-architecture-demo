using Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", "Demo");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.Id)
            .ValueGeneratedNever();

        builder.Property(order => order.CustomerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(order => order.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(order => order.Quantity)
            .IsRequired();

        builder.Property(order => order.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.TotalPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.Status)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(order => order.CreatedAt)
            .IsRequired();

        builder.Property(order => order.CancellationReason)
            .HasMaxLength(500);

        builder.Ignore(order => order.DomainEvents);
    }
}
