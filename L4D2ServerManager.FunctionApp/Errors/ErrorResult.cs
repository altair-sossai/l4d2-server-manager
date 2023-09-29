using System;
using System.Net;

namespace L4D2ServerManager.FunctionApp.Errors;

public class ErrorResult
{
    private ErrorResult(UnauthorizedAccessException unauthorizedAccessException)
    {
        StatusCode = HttpStatusCode.Unauthorized;
        Message = unauthorizedAccessException.Message;
    }

    private ErrorResult(Exception exception)
    {
        StatusCode = HttpStatusCode.InternalServerError;
        Message = exception.Message;
    }

    public HttpStatusCode StatusCode { get; }
    public string Message { get; }

    public static ErrorResult Build(Exception exception)
    {
        if (exception is AggregateException)
            exception = exception.InnerException;

        return exception switch
        {
            UnauthorizedAccessException unauthorizedAccessException => new ErrorResult(unauthorizedAccessException),
            _ => new ErrorResult(exception)
        };
    }
}