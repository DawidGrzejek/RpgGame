using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Common
{
    /// <summary>
    /// Represents an error that occurs during an operation, providing details about the error code, description,  and
    /// optionally the name of the property associated with the error.
    /// </summary>
    /// <remarks>This class is designed to encapsulate error information in a structured format, making it
    /// easier to handle and propagate errors in applications. It includes predefined factory methods for common error
    /// scenarios,  such as validation failures, unauthorized access, and business rule violations.</remarks>
    public sealed class OperationError
    {
        /// <summary>
        /// Gets the error code that uniquely identifies the type of error.
        /// </summary>
        public string Code { get; }
        /// <summary>
        /// Gets the human-readable description of the error.
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Gets the name of the property associated with the error, if applicable.
        /// </summary>
        public string? PropertyName { get; }

        public OperationError(string code, string description, string? propertyName = null)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            PropertyName = propertyName;
        }

        #region Predefined common errors
        public static OperationError NotFound(string entityName, string identifier) =>
            new($"{entityName}.NotFound", $"{entityName} with identifier '{identifier}' not found.");

        public static OperationError ValidationFailed(string propertyName, string errorMessage) =>
            new("Validation.Failed", errorMessage, propertyName);

        public static OperationError Unauthorized(string operation) =>
            new("Unauthorized", $"You are not authorized to perform operation: '{operation}'.");

        public static OperationError Conflict(string message) =>
            new("Operation.Conflict", message);

        public static OperationError InternalError(string message) =>
            new("Internal.Error", message);
        public static OperationError BusinessRuleViolation(string rule, string message) =>
            new($"BusinessRule.{rule}", message);

        #endregion

        #region overrides
        public override string ToString() => $"[{Code}] {Description}";

        public override bool Equals(object? obj) =>
            obj is OperationError error && Code == error.Code && Description == error.Description;

        public override int GetHashCode() => HashCode.Combine(Code, Description);

        #endregion
    }
}
