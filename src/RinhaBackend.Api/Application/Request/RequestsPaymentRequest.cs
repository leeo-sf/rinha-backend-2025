using MediatR;
using RinhaBackend.Api.Application;

namespace RinhaBackend.Api.Application.Request;

public record RequestsPaymentRequest(Guid CorrelationId, decimal Amount)
    : IRequest<Result>;