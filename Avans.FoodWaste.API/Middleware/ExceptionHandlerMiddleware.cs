using Avans.FoodWaste.Core.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Avans.FoodWaste.Core.Exceptions;


namespace Avans.FoodWaste.API.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Default status code

            var errorResponse = new ErrorResponseDto
            {
                Message = "An error occurred while processing your request.",
                Details = exception.Message // Add the exception message to Details
            };
            if (exception is ArgumentException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Invalid input provided. Please check your request parameters.";
                errorResponse.Details = exception.Message;
            }
            else if (exception is CafeteriaNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = "The specified cafeteria was not found.";
                errorResponse.Details = exception.Message;
            }
            else if (exception is HotMealsNotAvailableException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Hot meals are not available at the specified cafeteria.";
                errorResponse.Details = exception.Message;
            }
            else if (exception is InvalidProductException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "One or more of the specified products were not found.";
                errorResponse.Details = exception.Message; // This will contain the list of invalid IDs
            }
            else if (exception is InvalidDateTimeException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Invalid date or time provided. Please check your input.";
                errorResponse.Details = exception.Message; // This will contain the specific date/time error
            }
            else if (exception is PackageNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = "The specified package was not found.";
                errorResponse.Details = exception.Message;
            }

            // ... (Add more else if blocks for other custom exceptions as needed) ...

            return context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}