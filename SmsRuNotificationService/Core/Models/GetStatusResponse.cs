using System.Text.Json.Serialization;

namespace SmsRuNotificationService.Core.Models;

public class BaseResponse<TNotificationResponse>
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("sms")]
    public Dictionary<string, TNotificationResponse> Sms { get; set; } = new ();
}