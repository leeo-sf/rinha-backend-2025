using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Domain.Entity;
using RinhaBackend.Api.Domain.Enum;

namespace RinhaBackend.Api.Application.Extensions;

public static class PaymentsExtensions
{
    public static PaymentSummaryContract BuildSummary(this List<Payment> payments)
    {
        var paymentsProcessedByDefault = payments.Where(p => p.ProcessedBy == PaymentProcessorEnum.DEFAULT).ToList();
        var paymentsProcessedByFallback = payments.Where(p => p.ProcessedBy == PaymentProcessorEnum.FALLBACK).ToList();
        return new(new(paymentsProcessedByDefault.AmountOfPayments(), paymentsProcessedByDefault.TotalAmountOfPayments()),
            new(paymentsProcessedByFallback.AmountOfPayments(), paymentsProcessedByFallback.TotalAmountOfPayments()));
    }

    public static decimal TotalAmountOfPayments(this List<Payment> payments) =>
        payments.Sum(p => p.Amount);

    public static int AmountOfPayments(this List<Payment> payments) =>
        payments.Count;
}