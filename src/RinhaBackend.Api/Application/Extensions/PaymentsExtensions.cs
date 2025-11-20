using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Domain.Entity;
using RinhaBackend.Api.Domain.Enum;

namespace RinhaBackend.Api.Application.Extensions;

public static class PaymentsExtensions
{
    public static PaymentSummaryContract BuildSummary(this List<Payment> payments)
    {
        int defaultCount = 0, fallbackCount = 0;
        decimal defaultTotal = 0m, fallbackTotal = 0m;
        foreach (var payment in payments)
        {
            if (payment.ProcessedBy == PaymentProcessorEnum.DEFAULT)
            {
                defaultCount++;
                defaultTotal += payment.Amount;
            }
            else
            {
                fallbackCount++;
                fallbackTotal += payment.Amount;
            }
        }
        return new (
            new ProcessingSummaryContract(defaultCount, defaultTotal),
            new ProcessingSummaryContract(fallbackCount, fallbackTotal)
        );
    }
}