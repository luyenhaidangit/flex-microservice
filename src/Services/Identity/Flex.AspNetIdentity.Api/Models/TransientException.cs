namespace Flex.AspNetIdentity.Api.Models
{
    public sealed class TransientException : Exception
    {
        public TransientException(string message, Exception inner) : base(message, inner) { }
    }
}

