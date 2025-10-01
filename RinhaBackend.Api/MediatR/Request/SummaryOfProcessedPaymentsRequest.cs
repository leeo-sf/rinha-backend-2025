using MediatR;
using RinhaBackend.Api.Contract;

namespace RinhaBackend.Api.MediatR.Request;

public record SummaryOfProcessedPaymentsRequest(DateTime From, DateTime To)
    : IRequest<Result<PaymentsProcessedAtIntervalsContract>>;