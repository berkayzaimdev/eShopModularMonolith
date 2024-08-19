using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Shared.Exceptions.Handler;

public class GlobalExceptionHandler 
	(ILogger<GlobalExceptionHandler> logger)
	: IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
	{
		logger.LogError("Error Message: {exceptionMessage}, Time of occurence {time}", exception.Message, DateTime.UtcNow);

		string message = exception.Message;
		string typeName = exception.GetType().Name;

		(string Detail, string Title, int StatusCode) details = exception switch
		{
			InternalServerException =>
			(
				message,
				typeName,
				context.Response.StatusCode = StatusCodes.Status500InternalServerError
			),
			ValidationException =>
			(
				message,
				typeName,
				context.Response.StatusCode = StatusCodes.Status500InternalServerError
			),
			BadRequestException =>
			(
				message,
				typeName,
				context.Response.StatusCode = StatusCodes.Status400BadRequest
			),			
			NotFoundException =>
			(
				message,
				typeName,
				context.Response.StatusCode = StatusCodes.Status404NotFound
			),
			_ =>
			(
				message,
				typeName,
				context.Response.StatusCode = StatusCodes.Status500InternalServerError
			)
		};

		var problemDetails = new ProblemDetails
		{
			Title = details.Title,
			Detail = details.Detail,
			Status = details.StatusCode,
			Instance = context.Request.Path
		};

		problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

		if (exception is ValidationException validationException)
		{
			problemDetails.Extensions.Add("ValidationErrors", validationException.Errors);
		}

		await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
		return true;
	}
}
