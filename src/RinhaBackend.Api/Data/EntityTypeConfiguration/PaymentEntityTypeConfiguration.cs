using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinhaBackend.Api.Domain.Entity;

namespace RinhaBackend.Api.Data.EntityTypeConfiguration;

public class PaymentEntityTypeConfiguration
    : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(e => e.CorrelationId);
        builder.Property(e => e.CorrelationId).HasColumnName("correlationId").IsRequired().HasComment("Identificador");
        builder.Property(e => e.Amount).HasColumnName("amount").IsRequired().HasComment("Valor do pagamento");
        builder.Property(e => e.RequestedAt).HasColumnName("requestedAt").IsRequired(false).HasComment("Data e hora da solicitação");
        builder.Property(e => e.IsProcessed).HasColumnName("isProcessed").IsRequired().HasComment("Pagamento processado");
        builder.Property(e => e.ProcessedBy).HasColumnName("processedBy").IsRequired(false).HasComment("Onde o pagamento foi processado");
    }
}