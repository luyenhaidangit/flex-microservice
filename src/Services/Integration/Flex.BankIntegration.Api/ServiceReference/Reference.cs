﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IHOSTService")]
public interface IHOSTService
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHOSTService/DoWork", ReplyAction="http://tempuri.org/IHOSTService/DoWorkResponse")]
    System.Threading.Tasks.Task DoWorkAsync();
    
    // CODEGEN: Generating message contract since the operation has multiple return values.
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHOSTService/MessageByte", ReplyAction="http://tempuri.org/IHOSTService/MessageByteResponse")]
    System.Threading.Tasks.Task<MessageByteResponse> MessageByteAsync(MessageByteRequest request);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHOSTService/GetLastedVersionClient", ReplyAction="http://tempuri.org/IHOSTService/GetLastedVersionClientResponse")]
    System.Threading.Tasks.Task<byte[]> GetLastedVersionClientAsync();
    
    // CODEGEN: Generating message contract since the operation has multiple return values.
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHOSTService/MessageString", ReplyAction="http://tempuri.org/IHOSTService/MessageStringResponse")]
    System.Threading.Tasks.Task<MessageStringResponse> MessageStringAsync(MessageStringRequest request);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHOSTService/OMessageByte", ReplyAction="http://tempuri.org/IHOSTService/OMessageByteResponse")]
    [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[]))]
    System.Threading.Tasks.Task<object[]> OMessageByteAsync(byte[] pv_arrByteMessage);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHOSTService/OMessageString", ReplyAction="http://tempuri.org/IHOSTService/OMessageStringResponse")]
    [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[]))]
    System.Threading.Tasks.Task<object[]> OMessageStringAsync(string pv_strMessage);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHOSTService/GetFlagSignature", ReplyAction="http://tempuri.org/IHOSTService/GetFlagSignatureResponse")]
    System.Threading.Tasks.Task<string> GetFlagSignatureAsync();
    
    // CODEGEN: Generating message contract since the operation has multiple return values.
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHOSTService/GetTicketAccount", ReplyAction="http://tempuri.org/IHOSTService/GetTicketAccountResponse")]
    System.Threading.Tasks.Task<GetTicketAccountResponse> GetTicketAccountAsync(GetTicketAccountRequest request);
    
    // CODEGEN: Generating message contract since the operation has multiple return values.
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHOSTService/GetInfoAuthorMicrosoft", ReplyAction="http://tempuri.org/IHOSTService/GetInfoAuthorMicrosoftResponse")]
    System.Threading.Tasks.Task<GetInfoAuthorMicrosoftResponse> GetInfoAuthorMicrosoftAsync(GetInfoAuthorMicrosoftRequest request);
    
    // CODEGEN: Generating message contract since the operation has multiple return values.
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHOSTService/GetSecondsLimitAFK", ReplyAction="http://tempuri.org/IHOSTService/GetSecondsLimitAFKResponse")]
    System.Threading.Tasks.Task<GetSecondsLimitAFKResponse> GetSecondsLimitAFKAsync(GetSecondsLimitAFKRequest request);
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[System.ServiceModel.MessageContractAttribute(WrapperName="MessageByte", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
public partial class MessageByteRequest
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
    public byte[] pv_arrByteMessage;
    
    public MessageByteRequest()
    {
    }
    
    public MessageByteRequest(byte[] pv_arrByteMessage)
    {
        this.pv_arrByteMessage = pv_arrByteMessage;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[System.ServiceModel.MessageContractAttribute(WrapperName="MessageByteResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
public partial class MessageByteResponse
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
    public long MessageByteResult;
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
    public byte[] pv_arrByteMessage;
    
    public MessageByteResponse()
    {
    }
    
    public MessageByteResponse(long MessageByteResult, byte[] pv_arrByteMessage)
    {
        this.MessageByteResult = MessageByteResult;
        this.pv_arrByteMessage = pv_arrByteMessage;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[System.ServiceModel.MessageContractAttribute(WrapperName="MessageString", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
public partial class MessageStringRequest
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
    public string pv_strMessage;
    
    public MessageStringRequest()
    {
    }
    
    public MessageStringRequest(string pv_strMessage)
    {
        this.pv_strMessage = pv_strMessage;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[System.ServiceModel.MessageContractAttribute(WrapperName="MessageStringResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
public partial class MessageStringResponse
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
    public long MessageStringResult;
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
    public string pv_strMessage;
    
    public MessageStringResponse()
    {
    }
    
    public MessageStringResponse(long MessageStringResult, string pv_strMessage)
    {
        this.MessageStringResult = MessageStringResult;
        this.pv_strMessage = pv_strMessage;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[System.ServiceModel.MessageContractAttribute(WrapperName="GetTicketAccount", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
public partial class GetTicketAccountRequest
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
    public byte[] pv_arrByteMessage;
    
    public GetTicketAccountRequest()
    {
    }
    
    public GetTicketAccountRequest(byte[] pv_arrByteMessage)
    {
        this.pv_arrByteMessage = pv_arrByteMessage;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[System.ServiceModel.MessageContractAttribute(WrapperName="GetTicketAccountResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
public partial class GetTicketAccountResponse
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
    public long GetTicketAccountResult;
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
    public byte[] pv_arrByteMessage;
    
    public GetTicketAccountResponse()
    {
    }
    
    public GetTicketAccountResponse(long GetTicketAccountResult, byte[] pv_arrByteMessage)
    {
        this.GetTicketAccountResult = GetTicketAccountResult;
        this.pv_arrByteMessage = pv_arrByteMessage;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[System.ServiceModel.MessageContractAttribute(WrapperName="GetInfoAuthorMicrosoft", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
public partial class GetInfoAuthorMicrosoftRequest
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
    public byte[] pv_arrByteMessage;
    
    public GetInfoAuthorMicrosoftRequest()
    {
    }
    
    public GetInfoAuthorMicrosoftRequest(byte[] pv_arrByteMessage)
    {
        this.pv_arrByteMessage = pv_arrByteMessage;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[System.ServiceModel.MessageContractAttribute(WrapperName="GetInfoAuthorMicrosoftResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
public partial class GetInfoAuthorMicrosoftResponse
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
    public long GetInfoAuthorMicrosoftResult;
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
    public byte[] pv_arrByteMessage;
    
    public GetInfoAuthorMicrosoftResponse()
    {
    }
    
    public GetInfoAuthorMicrosoftResponse(long GetInfoAuthorMicrosoftResult, byte[] pv_arrByteMessage)
    {
        this.GetInfoAuthorMicrosoftResult = GetInfoAuthorMicrosoftResult;
        this.pv_arrByteMessage = pv_arrByteMessage;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[System.ServiceModel.MessageContractAttribute(WrapperName="GetSecondsLimitAFK", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
public partial class GetSecondsLimitAFKRequest
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
    public byte[] pv_arrByteMessage;
    
    public GetSecondsLimitAFKRequest()
    {
    }
    
    public GetSecondsLimitAFKRequest(byte[] pv_arrByteMessage)
    {
        this.pv_arrByteMessage = pv_arrByteMessage;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[System.ServiceModel.MessageContractAttribute(WrapperName="GetSecondsLimitAFKResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
public partial class GetSecondsLimitAFKResponse
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
    public long GetSecondsLimitAFKResult;
    
    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
    public byte[] pv_arrByteMessage;
    
    public GetSecondsLimitAFKResponse()
    {
    }
    
    public GetSecondsLimitAFKResponse(long GetSecondsLimitAFKResult, byte[] pv_arrByteMessage)
    {
        this.GetSecondsLimitAFKResult = GetSecondsLimitAFKResult;
        this.pv_arrByteMessage = pv_arrByteMessage;
    }
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
public interface IHOSTServiceChannel : IHOSTService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
public partial class HOSTServiceClient : System.ServiceModel.ClientBase<IHOSTService>, IHOSTService
{
    
    /// <summary>
    /// Implement this partial method to configure the service endpoint.
    /// </summary>
    /// <param name="serviceEndpoint">The endpoint to configure</param>
    /// <param name="clientCredentials">The client credentials</param>
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
    
    public HOSTServiceClient() : 
            base(HOSTServiceClient.GetDefaultBinding(), HOSTServiceClient.GetDefaultEndpointAddress())
    {
        this.Endpoint.Name = EndpointConfiguration.WSHttpBinding_IHOSTService.ToString();
        ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
    }
    
    public HOSTServiceClient(EndpointConfiguration endpointConfiguration) : 
            base(HOSTServiceClient.GetBindingForEndpoint(endpointConfiguration), HOSTServiceClient.GetEndpointAddress(endpointConfiguration))
    {
        this.Endpoint.Name = endpointConfiguration.ToString();
        ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
    }
    
    public HOSTServiceClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
            base(HOSTServiceClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
    {
        this.Endpoint.Name = endpointConfiguration.ToString();
        ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
    }
    
    public HOSTServiceClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(HOSTServiceClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
    {
        this.Endpoint.Name = endpointConfiguration.ToString();
        ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
    }
    
    public HOSTServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public System.Threading.Tasks.Task DoWorkAsync()
    {
        return base.Channel.DoWorkAsync();
    }
    
    public System.Threading.Tasks.Task<MessageByteResponse> MessageByteAsync(MessageByteRequest request)
    {
        return base.Channel.MessageByteAsync(request);
    }
    
    public System.Threading.Tasks.Task<byte[]> GetLastedVersionClientAsync()
    {
        return base.Channel.GetLastedVersionClientAsync();
    }
    
    public System.Threading.Tasks.Task<MessageStringResponse> MessageStringAsync(MessageStringRequest request)
    {
        return base.Channel.MessageStringAsync(request);
    }
    
    public System.Threading.Tasks.Task<object[]> OMessageByteAsync(byte[] pv_arrByteMessage)
    {
        return base.Channel.OMessageByteAsync(pv_arrByteMessage);
    }
    
    public System.Threading.Tasks.Task<object[]> OMessageStringAsync(string pv_strMessage)
    {
        return base.Channel.OMessageStringAsync(pv_strMessage);
    }
    
    public System.Threading.Tasks.Task<string> GetFlagSignatureAsync()
    {
        return base.Channel.GetFlagSignatureAsync();
    }
    
    public System.Threading.Tasks.Task<GetTicketAccountResponse> GetTicketAccountAsync(GetTicketAccountRequest request)
    {
        return base.Channel.GetTicketAccountAsync(request);
    }
    
    public System.Threading.Tasks.Task<GetInfoAuthorMicrosoftResponse> GetInfoAuthorMicrosoftAsync(GetInfoAuthorMicrosoftRequest request)
    {
        return base.Channel.GetInfoAuthorMicrosoftAsync(request);
    }
    
    public System.Threading.Tasks.Task<GetSecondsLimitAFKResponse> GetSecondsLimitAFKAsync(GetSecondsLimitAFKRequest request)
    {
        return base.Channel.GetSecondsLimitAFKAsync(request);
    }
    
    public virtual System.Threading.Tasks.Task OpenAsync()
    {
        return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
    }
    
    private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
    {
        if ((endpointConfiguration == EndpointConfiguration.WSHttpBinding_IHOSTService))
        {
            System.ServiceModel.WSHttpBinding result = new System.ServiceModel.WSHttpBinding();
            result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            result.MaxReceivedMessageSize = int.MaxValue;
            result.AllowCookies = true;
            result.Security.Mode = System.ServiceModel.SecurityMode.None;
            return result;
        }
        throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
    }
    
    private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
    {
        if ((endpointConfiguration == EndpointConfiguration.WSHttpBinding_IHOSTService))
        {
            return new System.ServiceModel.EndpointAddress("http://[::]:52514/HOSTService/HOSTService.svc");
        }
        throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
    }
    
    private static System.ServiceModel.Channels.Binding GetDefaultBinding()
    {
        return HOSTServiceClient.GetBindingForEndpoint(EndpointConfiguration.WSHttpBinding_IHOSTService);
    }
    
    private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
    {
        return HOSTServiceClient.GetEndpointAddress(EndpointConfiguration.WSHttpBinding_IHOSTService);
    }
    
    public enum EndpointConfiguration
    {
        
        WSHttpBinding_IHOSTService,
    }
}
