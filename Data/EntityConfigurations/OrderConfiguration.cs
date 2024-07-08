using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace Data.EntityConfigurations;

internal class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(order => order.Id);
        builder.HasIndex(order => order.Number).IsUnique();
        builder.Property(order => order.Date).IsRequired();
        builder.Property(order => order.Note).HasMaxLength(255);
    }
}