using Microsoft.AspNetCore.Mvc;
using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Application.Extensions;
using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Domain.Interface;

namespace RinhaBackend.Api.Presentation.Endpoints;

public static class PaymentEndpoints
{
    public static void AddPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/payments")
            .WithTags("Payment").WithOpenApi();

        group.MapPost(string.Empty, RequestsPaymentAsync)
            .Produces(StatusCodes.Status202Accepted);
        group.MapGet("payments-summary", ProcessingSummaryAsync)
            .Produces<PaymentSummaryContract>(StatusCodes.Status200OK);
    }
    private static async ValueTask<IResult> RequestsPaymentAsync(
        [FromBody] PaymentContract request,
        IPaymentQueueService queue)
    {
        await queue.EnqueueAsync(request);
        return Results.Accepted();
    }

    private static async ValueTask<IResult> ProcessingSummaryAsync(
        [AsParameters] SummaryOfProcessedPaymentsContract request,
        [FromServices] IPaymentsRepository paymentsRepository,
        CancellationToken cancellationToken)
    {
        var payments = await paymentsRepository.PaymentsProcessedAsync(request.From, request.To, cancellationToken);
        return Results.Ok(payments.BuildSummary());
    }
}