using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Common
{
    /// <summary>
    /// Represents the result of an operation, indicating success or failure and containing error information if applicable.
    /// </summary>
    public class OperationResult
    {
        private readonly List<OperationError> _errors;

        /// <summary>
        /// Gets a value indicating whether the operation succeeded.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// Gets the list of errors associated with the operation.
        /// </summary>
        public IReadOnlyList<OperationError> Errors => _errors.AsReadOnly();

        /// <summary>
        /// Gets the data returned by the operation, if any.
        /// </summary>
        public object? Data { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class for successful operations.
        /// </summary>
        /// <param name="succeeded">Indicates if the operation succeeded.</param>
        /// <param name="data">Optional data returned by the operation.</param>
        protected OperationResult(bool succeeded, object? data = null)
        {
            Succeeded = succeeded;
            Data = data;
            _errors = new List<OperationError>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class for failed operations with multiple errors.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <exception cref="ArgumentNullException">Thrown if errors is null.</exception>
        protected OperationResult(IEnumerable<OperationError> errors)
        {
            Succeeded = false;
            Data = null;
            _errors = errors?.ToList() ?? throw new ArgumentNullException(nameof(errors));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class for a failed operation with a single error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">Thrown if error is null.</exception>
        protected OperationResult(OperationError error)
        {
            Succeeded = false;
            Data = null;
            _errors = new List<OperationError> { error ?? throw new ArgumentNullException(nameof(error)) };
        }

        /// <summary>
        /// Creates a successful <see cref="OperationResult"/> with no data.
        /// </summary>
        public static OperationResult Success() => new(true);

        /// <summary>
        /// Creates a successful <see cref="OperationResult"/> with the specified data.
        /// </summary>
        /// <param name="data">The data to return.</param>
        public static OperationResult Success(object data) => new(true, data);

        /// <summary>
        /// Creates a failed <see cref="OperationResult"/> with a single error.
        /// </summary>
        /// <param name="error">The error.</param>
        public static OperationResult Failure(OperationError error) => new(error);

        /// <summary>
        /// Creates a failed <see cref="OperationResult"/> with multiple errors.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        public static OperationResult Failure(IEnumerable<OperationError> errors) => new(errors);

        /// <summary>
        /// Creates a failed <see cref="OperationResult"/> with a custom error code and description.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorDescription">The error description.</param>
        public static OperationResult Failure(string errorCode, string errorDescription) =>
            new(new OperationError(errorCode, errorDescription));

        /// <summary>
        /// Creates a failed <see cref="OperationResult"/> representing a not found error.
        /// </summary>
        /// <param name="entityName">The name of the entity.</param>
        /// <param name="identifier">The identifier of the entity.</param>
        public static OperationResult NotFound(string entityName, string identifier) =>
            Failure(OperationError.NotFound(entityName, identifier));

        /// <summary>
        /// Creates a failed <see cref="OperationResult"/> representing an unauthorized error.
        /// </summary>
        /// <param name="operation">The operation attempted.</param>
        public static OperationResult Unauthorized(string operation) =>
            Failure(OperationError.Unauthorized(operation));

        /// <summary>
        /// Creates a failed <see cref="OperationResult"/> representing a validation failure.
        /// </summary>
        /// <param name="propertyName">The name of the property that failed validation.</param>
        /// <param name="message">The validation error message.</param>
        public static OperationResult ValidationFailed(string propertyName, string message) =>
            Failure(OperationError.ValidationFailed(propertyName, message));

        /// <summary>
        /// Creates a failed <see cref="OperationResult"/> representing a business rule violation.
        /// </summary>
        /// <param name="rule">The business rule.</param>
        /// <param name="message">The error message.</param>
        public static OperationResult BusinessRuleViolation(string rule, string message) =>
            Failure(OperationError.BusinessRuleViolation(rule, message));

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        public bool Failed => !Succeeded;

        /// <summary>
        /// Gets the description of the first error, or an empty string if there are no errors.
        /// </summary>
        public string FirstErrorMessage => _errors.FirstOrDefault()?.Description ?? string.Empty;

        /// <summary>
        /// Gets a value indicating whether the operation has any errors.
        /// </summary>
        public bool HasErrors => _errors.Count > 0;

        /// <summary>
        /// Implicitly converts an <see cref="OperationResult"/> to a boolean indicating success.
        /// </summary>
        /// <param name="result">The operation result.</param>
        public static implicit operator bool(OperationResult result) => result.Succeeded;

        /// <summary>
        /// Returns a string representation of the operation result.
        /// </summary>
        public override string ToString()
        {
            if (Succeeded)
                return "Operation succeeded";

            return $"Operation failed: {string.Join(", ", _errors.Select(e => e.ToString()))}";
        }
    }

    /// <summary>
    /// Generic version of <see cref="OperationResult"/> for strongly-typed data.
    /// </summary>
    /// <typeparam name="T">The type of data returned by the operation.</typeparam>
    public class OperationResult<T> : OperationResult
    {
        /// <summary>
        /// Gets the strongly-typed data returned by the operation, if any.
        /// </summary>
        public new T? Data { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult{T}"/> class for successful operations.
        /// </summary>
        /// <param name="succeeded">Indicates if the operation succeeded.</param>
        /// <param name="data">The data returned by the operation.</param>
        private OperationResult(bool succeeded, T? data = default) : base(succeeded, data)
        {
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult{T}"/> class for failed operations with multiple errors.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        private OperationResult(IEnumerable<OperationError> errors) : base(errors)
        {
            Data = default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult{T}"/> class for a failed operation with a single error.
        /// </summary>
        /// <param name="error">The error.</param>
        private OperationResult(OperationError error) : base(error)
        {
            Data = default;
        }

        /// <summary>
        /// Creates a successful <see cref="OperationResult{T}"/> with the specified data.
        /// </summary>
        /// <param name="data">The data to return.</param>
        public static OperationResult<T> Success(T data) => new(true, data);

        /// <summary>
        /// Creates a successful <see cref="OperationResult{T}"/> with no data.
        /// </summary>
        public static OperationResult<T> Success() => new(true);

        /// <summary>
        /// Creates a failed <see cref="OperationResult{T}"/> with a single error.
        /// </summary>
        /// <param name="error">The error.</param>
        public static OperationResult<T> Failure(OperationError error) => new(error);

        /// <summary>
        /// Creates a failed <see cref="OperationResult{T}"/> with multiple errors.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        public static OperationResult<T> Failure(IEnumerable<OperationError> errors) => new(errors);

        /// <summary>
        /// Creates a failed <see cref="OperationResult{T}"/> with a custom error code and description.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorDescription">The error description.</param>
        public static OperationResult<T> Failure(string errorCode, string errorDescription) =>
            new(new OperationError(errorCode, errorDescription));

        /// <summary>
        /// Creates a failed <see cref="OperationResult{T}"/> representing a not found error.
        /// </summary>
        /// <param name="entityName">The name of the entity.</param>
        /// <param name="identifier">The identifier of the entity.</param>
        public static OperationResult<T> NotFound(string entityName, string identifier) =>
            Failure(OperationError.NotFound(entityName, identifier));

        /// <summary>
        /// Creates a failed <see cref="OperationResult{T}"/> representing an unauthorized error.
        /// </summary>
        /// <param name="operation">The operation attempted.</param>
        public static OperationResult<T> Unauthorized(string operation) =>
            Failure(OperationError.Unauthorized(operation));

        /// <summary>
        /// Creates a failed <see cref="OperationResult{T}"/> representing a validation failure.
        /// </summary>
        /// <param name="propertyName">The name of the property that failed validation.</param>
        /// <param name="message">The validation error message.</param>
        public static OperationResult<T> ValidationFailed(string propertyName, string message) =>
            Failure(OperationError.ValidationFailed(propertyName, message));

        /// <summary>
        /// Creates a failed <see cref="OperationResult{T}"/> representing a business rule violation.
        /// </summary>
        /// <param name="rule">The business rule.</param>
        /// <param name="message">The error message.</param>
        public static OperationResult<T> BusinessRuleViolation(string rule, string message) =>
            Failure(OperationError.BusinessRuleViolation(rule, message));

        /// <summary>
        /// Implicitly converts an <see cref="OperationResult{T}"/> to its data value.
        /// </summary>
        /// <param name="result">The operation result.</param>
        public static implicit operator T?(OperationResult<T> result) => result.Data;

        /// <summary>
        /// Implicitly converts a value of type <typeparamref name="T"/> to a successful <see cref="OperationResult{T}"/>.
        /// </summary>
        /// <param name="data">The data value.</param>
        public static implicit operator OperationResult<T>(T data) => Success(data);

        /// <summary>
        /// Converts a non-generic <see cref="OperationResult"/> to a generic <see cref="OperationResult{T}"/>.
        /// </summary>
        /// <param name="result">The non-generic operation result.</param>
        public static OperationResult<T> FromResult(OperationResult result)
        {
            if (result.Succeeded && result.Data is T data)
                return Success(data);

            return result.Succeeded
                ? Success()
                : Failure(result.Errors);
        }
    }
}
