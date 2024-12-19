using Flex.Shared.Enums.General;
using Flex.Shared.Extensions;
using System.Text.Json.Serialization;

namespace Flex.Shared.SeedWork.General
{
    public class ApproveRequest<T>
    {
        public T Id { get; set; }

        private string _requestType;
        public string RequestType
        {
            get => _requestType;
            set
            {
                _requestType = value;
                RequestTypeEnum = EnumExtension.FromValue<ERequestType>(value);
            }
        }

        [JsonIgnore]
        public ERequestType RequestTypeEnum { get; private set; }
    }
}
