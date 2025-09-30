using MediatR;
using RinhaBackend.Api.MediatR;

namespace RinhaBackend.Api.MediatR.Request;

public record RequestsPaymentRequest(Guid CorrelationId, decimal Amount)
    : IRequest<Result>;