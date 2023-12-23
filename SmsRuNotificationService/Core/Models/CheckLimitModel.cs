using System.Text.Json.Serialization;

namespace SmsRuNotificationService.Core.Models;

public class CheckLimitModel
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }
    [JsonPropertyName("total_limit")]
    public int TotalLimit { get; set; }
    [JsonPropertyName("used_today")]
    public int UsedToday { get; set; }
}