using MediatR;
using RinhaBackend.Api.Application;
using RinhaBackend.Api.Application.Contract;

namespace RinhaBackend.Api.Application.Request;

public record SummaryOfProcessedPaymentsRequest(DateTime? From, DateTime? To)
    : IRequest<Result<PaymentsProcessedAtIntervalsContract>>;