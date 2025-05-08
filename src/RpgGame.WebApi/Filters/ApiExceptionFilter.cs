using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace RpgGame.WebApi.Filters
{
    /// <summary>
    /// A filter that handles exceptions thrown during the execution of an action in the Web API.
    /// </summary>
    /// <remarks>
    /// This filter provides centralized exception handling for the application. It maps specific exception types
    /// to appropriate HTTP responses and logs unhandled exceptions.
    /// </remarks>
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
                {
                    { typeof(ValidationException), HandleValidationException },
                    { typeof(NotFoundException), HandleNotFoundException },
                    { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException }
                };
        }

        /// <summary>
        /// Called when an exception occurs during the execution of an action.
        /// </summary>
        /// <param name="context">The exception context.</param>
        public override void OnException(ExceptionContext context)
        {
            HandleException(context);
            base.OnException(context);
        }

        /// <summary>
        /// Handles the exception by invoking the appropriate handler based on the exception type.
        /// </summary>
        /// <param name="context">The exception context.</param>
        private void HandleException(ExceptionContext context)
        {
            var type = context.Exception.GetType();

            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            HandleUnknownException(context);
        }

        /// <summary>
        /// Handles exceptions of type <see cref="ValidationException"/>.
        /// </summary>
        /// <param name="context">The exception context.</param>
        private void HandleValidationException(ExceptionContext context)
        {
            var exception = (ValidationException)context.Exception;

            var details = new ValidationProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred."
            };

            foreach (var error in exception.Errors)
            {
                string propertyName = error.PropertyName;
                details.Errors.Add(propertyName, new[] { error.ErrorMessage });
            }

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }

        /// <summary>
        /// Handles exceptions of type <see cref="NotFoundException"/>.
        /// </summary>
        /// <param name="context">The exception context.</param>
        private void HandleNotFoundException(ExceptionContext context)
        {
            var exception = (NotFoundException)context.Exception;

            var details = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "The specified resource was not found.",
                Detail = exception.Message
            };

            context.Result = new NotFoundObjectResult(details);
            context.ExceptionHandled = true;
        }

        /// <summary>
        /// Handles exceptions of type <see cref="UnauthorizedAccessException"/>.
        /// </summary>
        /// <param name="context">The exception context.</param>
        private void HandleUnauthorizedAccessException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            context.ExceptionHandled = true;
        }

        /// <summary>
        /// Handles exceptions that are not explicitly handled by other methods.
        /// </summary>
        /// <param name="context">The exception context.</param>
        private void HandleUnknownException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An unhandled exception occurred");

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }

    public class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NotFoundException(string message) : base(message)
        {
        }
    }
}