using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace Data.EntityConfigurations;

internal class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(item => item.Id);
        builder.HasIndex(item => item.Number).IsUnique();
        builder.Property(item => item.Ware).IsRequired();
        builder.Property(item => item.Order).IsRequired();
    }
}