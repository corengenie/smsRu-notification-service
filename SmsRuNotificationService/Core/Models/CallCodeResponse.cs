using System.Text.Json.Serialization;

namespace SmsRuNotificationService.Core.Models;

public class CallCodeResponse : SmsResponseInfo
{
    [JsonPropertyName("status_text")]
    public string? StatusText { get; set; }
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    [JsonPropertyName("cost")]
    public double Cost { get; set; }
    [JsonPropertyName("balance")]
    public double Balance { get; set; }
}