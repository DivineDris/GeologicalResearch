using System;
using System.Net;
using System.Text.Json;
using GeologicalResearch.Dto;
using GeologicalResearch.Exceptions;

namespace GeologicalResearch.Middleware;
//middleware для отлова ошибок
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (GeologicalResearchAppException ex)
        {
            await HandleExeptionAsync(context,
                                    ex.Message,
                                    ex.HttpStatusCode,
                                    ex.UserMessage);
        }
        catch (Exception ex)
        {
            await HandleExeptionAsync(context, 
                                    ex.Message, 
                                    HttpStatusCode.InternalServerError,
                                    "Internal server error");
        }
    }

    private async Task HandleExeptionAsync(HttpContext context, string exMessage, HttpStatusCode httpStatusCode, string message)
    {
        _logger.LogError(exMessage);
        HttpResponse response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)httpStatusCode;
        ErrorDto errorDto = new
        (
            (int)httpStatusCode,
            message
        );
        
        string result = errorDto.ToString();

        await response.WriteAsJsonAsync(result);
    }
}
