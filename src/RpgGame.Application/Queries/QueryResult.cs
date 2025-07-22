using RpgGame.Domain.Common;
using System.Collections.Generic;

namespace RpgGame.Application.Queries
{
    public class QueryResult<T> : OperationResult
    {
        public string Message { get; }
        public new T? Data => (T?)base.Data;

        protected QueryResult(bool success, string message, T? data)
            : base(success, data)
        {
            Message = message;
        }

        public static QueryResult<T> Ok(T data, string message = "Query executed successfully")
        {
            return new QueryResult<T>(true, message, data);
        }

        public static QueryResult<T> Fail(string message, IEnumerable<OperationError>? errors = null)
        {
            var result = new QueryResult<T>(false, message, default);
            if (errors != null)
            {
                foreach (var error in errors)
                    result.AddError(error);
            }
            return result;
        }
    }

    // Specialized version for collections with pagination
    public class PagedQueryResult<T> : QueryResult<IReadOnlyList<T>>
    {
        public int TotalCount { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public bool HasNextPage { get; }
        public bool HasPreviousPage { get; }

        private PagedQueryResult(
            bool success,
            string message,
            IReadOnlyList<T> data,
            int totalCount,
            int pageNumber,
            int pageSize)
            : base(success, message, data)
        {
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            HasNextPage = PageNumber < TotalPages;
            HasPreviousPage = PageNumber > 1;
        }

        public static PagedQueryResult<T> Ok(
            IReadOnlyList<T> data,
            int totalCount,
            int pageNumber,
            int pageSize,
            string message = "Query executed successfully")
        {
            return new PagedQueryResult<T>(true, message, data, totalCount, pageNumber, pageSize);
        }

        public static new PagedQueryResult<T> Fail(string message, IEnumerable<OperationError>? errors = null)
        {
            var result = new PagedQueryResult<T>(false, message, Array.Empty<T>(), 0, 0, 10);
            if (errors != null)
            {
                foreach (var error in errors)
                    result.AddError(error);
            }
            return result;
        }
    }
}