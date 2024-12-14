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

        public Result(bool isSuccess, string message, object? data = default, object? errors = default)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
            Errors = errors;
        }

        public static Result Success(string message = Constants.Message.Success, object? data = default)
        {
            return new Result(true, message, data);
        }

        public static Result Failure(string message = Constants.Message.Failure, object? errors = default)
        {
            return new Result(false, message, default, errors);
        }
    }
}