using System;
using hmsapi.Models;
using hmsapi.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace hmsapi.Middlewares
{
    internal sealed class NullReferenceExceptionHandler : IExceptionHandler
    {

        public NullReferenceExceptionHandler()
        {

        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if(exception is not NullReferenceException && exception is not KeyNotFoundException) {
                return false;
            }

            JLogger.WRT(new DaoJLogger()
            {
                LogEventLevel = Serilog.Events.LogEventLevel.Warning,
                FileName = $"ERR_LOG",
                Message = $"Message: {exception.Message} Stacktrace: {exception.StackTrace}\n"
            });

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status406NotAcceptable,
                Title = "Invalid request",
                Type = "Error",
                Detail = JsonConvert.SerializeObject(new DaoResponse()
                {
                    Status = false,
                    Message = "Missing paramters",
                })
                
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}

