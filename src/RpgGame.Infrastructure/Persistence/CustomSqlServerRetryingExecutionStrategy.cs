using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Data.SqlClient;

namespace RpgGame.Infrastructure.Persistence
{
    /// <summary>
    /// Custom SQL Server execution strategy that excludes authentication/authorization errors from retry logic
    /// to prevent IP blocking due to repeated failed login attempts
    /// </summary>
    public class CustomSqlServerRetryingExecutionStrategy : SqlServerRetryingExecutionStrategy
    {
        private static readonly int[] AuthenticationErrorNumbers = new[]
        {
            18456,  // Login failed for user
            18457,  // Login failed for user (client-side)
            18458,  // Login failed for user (server name not found)
            18459,  // Login failed for user (password expired)
            18460,  // Login failed for user (password must be changed)
            18461,  // Login failed for user (password policy violation)
            18486,  // Login failed for user (account locked)
            40615,  // Cannot open server specified in connection string
            2,      // A network-related or instance-specific error (often auth-related)
            4060,   // Cannot open database requested by the login
            18452   // Login failed (untrusted domain)
        };

        public CustomSqlServerRetryingExecutionStrategy(DbContext context) : base(context)
        {
        }

        public CustomSqlServerRetryingExecutionStrategy(ExecutionStrategyDependencies dependencies) : base(dependencies)
        {
        }

        public CustomSqlServerRetryingExecutionStrategy(
            DbContext context,
            int maxRetryCount,
            TimeSpan maxRetryDelay,
            ICollection<int>? errorNumbersToAdd) : base(context, maxRetryCount, maxRetryDelay, errorNumbersToAdd)
        {
        }

        public CustomSqlServerRetryingExecutionStrategy(
            ExecutionStrategyDependencies dependencies,
            int maxRetryCount,
            TimeSpan maxRetryDelay,
            ICollection<int>? errorNumbersToAdd) : base(dependencies, maxRetryCount, maxRetryDelay, errorNumbersToAdd)
        {
        }

        protected override bool ShouldRetryOn(Exception exception)
        {
            // Check if it's an authentication/authorization error
            if (exception is SqlException sqlException)
            {
                if (AuthenticationErrorNumbers.Contains(sqlException.Number))
                {
                    // Don't retry authentication errors
                    return false;
                }
            }

            // For all other exceptions, use the default retry logic
            return base.ShouldRetryOn(exception);
        }
    }
}