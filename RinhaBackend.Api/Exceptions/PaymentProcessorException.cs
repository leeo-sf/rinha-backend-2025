namespace RinhaBackend.Api.Exceptions;

public class PaymentProcessorException : Exception
{
    public PaymentProcessorException(string message)
        : base(message) { }
}