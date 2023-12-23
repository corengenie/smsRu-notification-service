using System.Runtime.Serialization;

namespace SmsRuNotificationService.Exceptions;

/// <summary>
/// Exception got by SmsRu
/// </summary>
[Serializable]
public class SmsRuException : Exception
{
    public SmsRuException() { }
    public SmsRuException(string message) : base(message) { }
    public SmsRuException(string message, Exception innerException) : base(message, innerException) { }
    protected SmsRuException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}