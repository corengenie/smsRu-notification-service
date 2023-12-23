using SmsRuNotificationService.Core.Models;

namespace SmsRuNotificationService.Core;

public interface ISmsSender
{
    public Task<SmsResponse> SendSmsAsync(string phone, string message, string? ip = null);
    public Task<double> GetBalanceAsync();
    public Task<string?> GetStatusAsync(string smsId);
    public Task<double> CheckCostAsync(string phone, string message);
    public Task<CallCodeResponse> SendCallCodeAsync(string phone, string? ip = null);
    public Task<CheckLimitModel> GetLimitAsync();
    public Task<bool> AuthIsValidAsync();
    public string PreparePhone(string phone);
}