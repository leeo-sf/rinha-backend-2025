namespace RinhaBackend.Api.Application.Contract;

public record PaymentSummaryContract(ProcessingSummaryContract Default, ProcessingSummaryContract Fallback);