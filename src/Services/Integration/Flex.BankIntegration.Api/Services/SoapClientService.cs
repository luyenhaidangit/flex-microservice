using System.ServiceModel;

namespace Flex.BankIntegration.Api.Services
{
    public class SoapClientService : IDisposable
    {
        private readonly HOSTServiceClient _client;

        public SoapClientService(string serviceUrl)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                AllowCookies = true,
                Security = { Mode = BasicHttpSecurityMode.Transport } // Bỏ security nếu không cần
            };

            var endpoint = new EndpointAddress(serviceUrl);
            _client = new HOSTServiceClient(binding, endpoint);
        }

        public Task<MessageByteResponse> CallMessageByteAsync(MessageByteRequest request) => _client.MessageByteAsync(request);

        public Task<string> GetFlagSignatureAsync() => _client.GetFlagSignatureAsync();

        public void Dispose()
        {
            if (_client.State == CommunicationState.Opened)
            {
                _client.Close();
            }
            _client.Abort();
        }
    }
}
