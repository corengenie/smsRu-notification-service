using System.Text.Json.Serialization;

namespace SmsRuNotificationService.Core.Models;

public class CheckCostResponseInfo
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }
    [JsonPropertyName("status_text")]
    public string? StatusText { get; set; }
    [JsonPropertyName("cost")]
    public double Cost { get; set; }
    [JsonPropertyName("sms")]
    public int Sms { get; set; }
}