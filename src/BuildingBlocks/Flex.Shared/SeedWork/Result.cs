using System.Text.Json.Serialization;

namespace Flex.Shared.SeedWork
{
    public class Result
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }

        public Result(bool isSuccess, string message, object? data = default)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }

        public static Result Success(string message = Constants.Message.Success, object? data = default)
        {
            return new Result(true, message, data);
        }

        public static Result Failure(string message = Constants.Message.Failure)
        {
            return new Result(false, message);
        }
    }
}
