namespace RpgGame.Application.Commands
{
    public class CommandResult
    {
        public bool Success { get; }
        public string Message { get; }
        public object Data { get; }

        // Change the constructor's access modifier from private to protected
        protected CommandResult(bool success, string message, object data = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static CommandResult Ok(string message = "Operation completed successfully", object data = null)
        {
            return new CommandResult(true, message, data);
        }

        public static CommandResult Fail(string message)
        {
            return new CommandResult(false, message);
        }
    }

    // Generic version for strongly typed results
    public class CommandResult<T> : CommandResult
    {
        public new T Data { get; }

        private CommandResult(bool success, string message, T data)
            : base(success, message, data)
        {
            Data = data;
        }

        public static CommandResult<T> Ok(T data, string message = "Operation completed successfully")
        {
            return new CommandResult<T>(true, message, data);
        }

        public static new CommandResult<T> Fail(string message)
        {
            return new CommandResult<T>(false, message, default);
        }
    }
}