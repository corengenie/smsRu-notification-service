using System.Runtime.Serialization;

namespace SmsRuNotificationService.Exceptions;

/// <summary>
/// SmsRu client configuration exception
/// </summary>
[Serializable]
public class SmsRuClientConfigurationException : Exception
{
    public SmsRuClientConfigurationException() { }
    public SmsRuClientConfigurationException(string message) : base(message) { }
    public SmsRuClientConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    protected SmsRuClientConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}