namespace RinhaBackend.Api.Contract;

public record PaymentsProcessedAtIntervalsContract(
    ProcessingSummaryContract Default, ProcessingSummaryContract Fallback);