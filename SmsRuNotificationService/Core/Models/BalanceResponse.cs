using System.Text.Json.Serialization;

namespace SmsRuNotificationService.Core.Models;

public class BalanceResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }
    [JsonPropertyName("balance")]
    public double Balance { get; set; }
}