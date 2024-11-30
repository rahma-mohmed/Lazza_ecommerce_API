﻿public class ExceptionMiddleware
{
	private readonly RequestDelegate _next;

	public ExceptionMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext httpContext)
	{
		try
		{
			await _next(httpContext);
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(httpContext, ex);
		}
	}

	private static Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

		var response = new
		{
			StatusCode = context.Response.StatusCode,
			Message = "An error occurred while processing your request.",
			Detailed = exception.Message 
		};

		return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
	}
}
