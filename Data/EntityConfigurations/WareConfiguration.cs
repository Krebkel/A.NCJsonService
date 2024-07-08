using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace Data.EntityConfigurations;

internal class WareConfiguration : IEntityTypeConfiguration<Ware>
{
    public void Configure(EntityTypeBuilder<Ware> builder)
    {
        builder.HasKey(ware => ware.Id);
        builder.HasIndex(ware => ware.Name).IsUnique();
        builder.Property(ware => ware.Value).IsRequired();
        builder.Property(ware => ware.Property).HasMaxLength(255);
    }
}