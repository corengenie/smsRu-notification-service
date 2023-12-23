using System.Text.Json.Serialization;

namespace SmsRuNotificationService.Core.Models;

public class SmsResponse : SmsResponseInfo
{
    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }
    [JsonPropertyName("status_text")]
    public string? StatusText { get; set; }
}