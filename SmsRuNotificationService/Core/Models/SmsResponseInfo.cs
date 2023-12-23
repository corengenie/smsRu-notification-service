using System.Text.Json.Serialization;

namespace SmsRuNotificationService.Core.Models;

public abstract class SmsResponseInfo
{
    public Guid Id { get; set; }
    [JsonPropertyName("sms_id")]
    public string? SmsId { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
}