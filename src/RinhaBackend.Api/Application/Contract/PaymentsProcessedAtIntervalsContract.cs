namespace RinhaBackend.Api.Application.Contract;

public record PaymentsProcessedAtIntervalsContract(
    ProcessingSummaryContract Default, ProcessingSummaryContract Fallback);