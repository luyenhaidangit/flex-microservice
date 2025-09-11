using FluentValidation.Results;

namespace Flex.Infrastructure.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public ValidationException() : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
            ErrorCode = "";
        }

        public ValidationException(string code) : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
            ErrorCode = code;
        }

        public ValidationException(IEnumerable<ValidationFailure> failures) : base("One or more validation failures have occurred.")
        {
            Errors = failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage).ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
            ErrorCode = "";
        }

        public string ErrorCode { get; }
        public IDictionary<string, string[]> Errors { get; }
    }
}
