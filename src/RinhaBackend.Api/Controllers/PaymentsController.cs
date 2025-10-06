using MediatR;
using Microsoft.AspNetCore.Mvc;
using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Application.Request;
using RinhaBackend.Api.Controllers.Base;
using System.ComponentModel.DataAnnotations;

namespace RinhaBackend.Api.Controllers;

[Route("api/payments")]
[ApiController]
public class PaymentsController
    : BaseController
{
    public PaymentsController(IMediator mediator)
        : base(mediator) { }

    [HttpPost]
    public async Task<ActionResult> RequestsPaymentAsync([Required] RequestsPaymentRequest request)
        => await SendCommand(request);

    [HttpGet("payments-summary")]
    public async Task<ActionResult<PaymentsProcessedAtIntervalsContract>> ProcessingSummaryAsync([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => await SendCommand(new SummaryOfProcessedPaymentsRequest(from, to));
}