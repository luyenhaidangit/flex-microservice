using System.Text.Json.Serialization;

namespace Flex.Shared.SeedWork
{
    public class Result
    {
        [JsonPropertyOrder(1)]
        public bool IsSuccess { get; set; }

        [JsonPropertyOrder(2)]
        public string Message { get; set; }

        [JsonPropertyOrder(3)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }

        [JsonPropertyOrder(4)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Errors { get; set; }

        [JsonPropertyOrder(5)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorCode { get; set; }

        public Result(bool isSuccess, string message, object? data = default, object? errors = default, string? errorCode = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
            Errors = errors;
            ErrorCode = errorCode;
        }

        public static Result Success(object? data = default, string message = Constants.Message.Success, string? errorCode = null)
        {
            return new Result(true, message, data, errorCode: errorCode);
        }

        public static Result Failure(object? errors = default, string message = Constants.Message.Failure, string? errorCode = null)
        {
            return new Result(false, message, default, errors, errorCode);
        }

        // Convenience methods with common error codes
        public static Result BadRequest(string message, object? errors = null)
        {
            return new Result(false, message, default, errors, Constants.ErrorCode.BadRequest);
        }

        public static Result Unauthorized(string message = "Unauthorized", object? errors = null)
        {
            return new Result(false, message, default, errors, Constants.ErrorCode.Unauthorized);
        }

        public static Result Forbidden(string message = "Forbidden", object? errors = null)
        {
            return new Result(false, message, default, errors, Constants.ErrorCode.Forbidden);
        }

        public static Result NotFound(string message, object? errors = null)
        {
            return new Result(false, message, default, errors, Constants.ErrorCode.NotFound);
        }

        public static Result Conflict(string message, object? errors = null)
        {
            return new Result(false, message, default, errors, Constants.ErrorCode.Conflict);
        }

        public static Result ValidationError(string message, object? errors = null)
        {
            return new Result(false, message, default, errors, Constants.ErrorCode.ValidationError);
        }

        public static Result InternalServerError(string message, object? errors = null)
        {
            return new Result(false, message, default, errors, Constants.ErrorCode.InternalServerError);
        }

        public static Result ServiceUnavailable(string message, object? errors = null)
        {
            return new Result(false, message, default, errors, Constants.ErrorCode.ServiceUnavailable);
        }
    }
}