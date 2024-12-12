namespace Flex.Identity.Infrastructure.Common.Results
{
    public class ApiErrorResult<T> : ApiResult<T>
    {
        public ApiErrorResult(string? message)
            : base(false, message)
        {
        }

        public ApiErrorResult(List<string> errors)
            : base(false)
        {
            Errors = errors;
        }

        public List<string> Errors { set; get; }
    }
}
