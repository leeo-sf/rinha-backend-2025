using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinhaBackend.Api.Data.Entity;

namespace RinhaBackend.Api.Config;

public class PaymentEntityTypeConfiguration
    : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(e => e.CorrelationId);
        builder.Property(e => e.CorrelationId).HasColumnName("correlationId").IsRequired().HasComment("Identificador");
        builder.Property(e => e.Amount).HasColumnName("amount").IsRequired().HasComment("Valor do pagamento");
        builder.Property(e => e.RequestedAt).HasColumnName("requestedAt").HasComment("Data e hora da solicitação");
    }
}