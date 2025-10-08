using Microsoft.AspNetCore.Mvc;
using RinhaBackend.Api.Application.Config;
using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Application.Extensions;
using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Application.Request;
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
            .Produces<PaymentsProcessedAtIntervalsContract>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> RequestsPaymentAsync(
        [FromBody] RequestsPaymentRequest request,
        [FromServices] AppConfiguration appConfiguration,
        [FromServices] IRabbitMQService rabbitMQService)
    {
        await rabbitMQService.PublisherAsync(appConfiguration.RabbitMQ, request, appConfiguration.RabbitMQ.Queues.PaymentRequestedQueue);
        return Results.Accepted();
    }

    private static async Task<IResult> ProcessingSummaryAsync(
        [AsParameters] SummaryOfProcessedPaymentsRequest request,
        [FromServices] IPaymentsRepository paymentsRepository,
        CancellationToken cancellationToken)
    {
        var payments = await paymentsRepository.PaymentsProcessedAsync(request.From, request.To, cancellationToken);
        return Results.Ok(payments.BuildSummary());
    }
}