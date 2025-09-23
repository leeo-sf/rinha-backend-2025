using MediatR;
using Microsoft.AspNetCore.Mvc;
using RinhaBackend.Api.Exceptions;
using RinhaBackend.Api.MediatR;

namespace RinhaBackend.Api.Controllers.Base;

[Route("api/[controller]")]
[ApiController]
public class BaseController(IMediator mediator)
    : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    protected async Task<ActionResult<T>> SendCommand<T>(IRequest<Result<T>> request, int statusCode = 200)
    {
        var response = await _mediator.Send(request);
        return response.IsSuccess
            ? StatusCode(statusCode, response.Value)
            : HandleError(response.Exception!);
    }

    protected async Task<ActionResult> SendCommand(
        IRequest<Result> request, int statusCode = 200)
    {
        var response = await _mediator.Send(request);
        return response.IsSuccess 
            ? StatusCode(statusCode) 
            : HandleError(response.Exception!);
    }

    protected ActionResult HandleError(Exception error) =>
        error switch
        {
            PaymentProcessorException
                => StatusCode(StatusCodes.Status422UnprocessableEntity, new { error.Message }),
            KeyNotFoundException
                => StatusCode(StatusCodes.Status404NotFound, new { error.Message }),
            _ => StatusCode(500)
        };
}