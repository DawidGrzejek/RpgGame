using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Log the request
            _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);

            try
            {
                var response = await next();

                // Log the success
                _logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);

                return response;
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "Error handling {RequestName}", typeof(TRequest).Name);
                throw;
            }
        }
    }
}