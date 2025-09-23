using MediatR;
using RinhaBackend.Api.MediatR;

namespace RinhaBackend.Api.Request;

public record RequestsPaymentRequest(Guid correlationId, decimal Amount)
    : IRequest<Result>;