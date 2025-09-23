using MediatR;
using RinhaBackend.Api.MediatR;
using RinhaBackend.Api.Request;

namespace RinhaBackend.Api.Handler;

public class PaymentsHandler
    : IRequestHandler<RequestsPaymentRequest, Result>
{
    public Task<Result> Handle(RequestsPaymentRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}