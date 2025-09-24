using MediatR;
using Microsoft.AspNetCore.Mvc;
using RinhaBackend.Api.Controllers.Base;
using RinhaBackend.Api.MediatR.Request;
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
    public async Task<IActionResult> RequestsPaymentAsync([Required] RequestsPaymentRequest request)
        => await SendCommand(request);
}