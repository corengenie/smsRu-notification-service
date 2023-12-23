using System.Text.Json.Serialization;

namespace SmsRuNotificationService.Core.Models;

public class CheckCostResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }
    [JsonPropertyName("sms")]
    public Dictionary<string, CheckCostResponseInfo> Sms { get; set; } = new();
    [JsonPropertyName("total_cost")]
    public double TotalCost { get; set; }
    [JsonPropertyName("total_sms")]
    public int TotalSms { get; set; }
}