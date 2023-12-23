namespace SmsRuNotificationService.Configuration;

public class SmsRuSenderOptions
{
    /// <summary>
    /// Launch in test mode
    /// </summary>
    public bool Test { get; set; } = false;
    /// <summary>
    /// Api_id token for smsRu auth
    /// </summary>
    public string ApiId { get; set; } = null!;
    /// <summary>
    /// Sender name. If null, name from smsRu profile will be used
    /// </summary>
    public string? From { get; set; }
    /// <summary>
    /// Max length of message without cyrillic characters
    /// </summary>
    public int MaxMessageLength { get; set; }
    /// <summary>
    /// Max cost of message
    /// </summary>
    public double MaxMessageCost { get; set; }
}