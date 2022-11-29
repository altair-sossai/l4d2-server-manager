using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;

namespace L4D2ServerManager.FunctionApp.Errors;

public class ErrorResult
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    private ErrorResult(ValidationException validationException)
    {
        StatusCode = HttpStatusCode.BadRequest;
        Message = validationException.Message;
        Errors = validationException.Errors?.Select(failure => new Error(failure)).ToList();
    }

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
    public List<Error> Errors { get; } = new();

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, Options);
    }

    public static ErrorResult Build(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => new ErrorResult(validationException),
            UnauthorizedAccessException unauthorizedAccessException => new ErrorResult(unauthorizedAccessException),
            _ => new ErrorResult(exception)
        };
    }
}