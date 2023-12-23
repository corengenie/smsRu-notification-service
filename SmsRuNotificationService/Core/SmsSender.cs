using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Iuliia;
using SmsRuNotificationService.Configuration;
using SmsRuNotificationService.Core.Helpers;
using SmsRuNotificationService.Core.Models;
using SmsRuNotificationService.Exceptions;

namespace SmsRuNotificationService.Core;

public class SmsSender : ISmsSender
{
    private readonly SmsRuSenderOptions _options;
    private const string BaseUrl = "https://sms.ru/";
#pragma warning disable SYSLIB1045
    private readonly Regex _phoneRegEx = new ("^[0-9]{11}$");
    private readonly Regex _anyRussianCharacterRegEx = new(@"[а-яА-ЯёЁйЙ]");
    private readonly Regex _anySpacesCharacterRegEx = new("\\s+");
    private readonly Regex _ipRegex = new("^((25[0-5]|(2[0-4]|1\\d|[1-9]|)\\d)(\\.(?!$)|$)){4}$");
#pragma warning restore SYSLIB1045
    private readonly HttpClient _httpClient = new ();
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public SmsSender(Action<SmsRuSenderOptions> options)
    {
        _options = new SmsRuSenderOptions();
        options.Invoke(_options);
        _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        _jsonSerializerOptions.Converters.Insert(0, new AutoNumberToStringConverter());
    }
    
    public async Task<SmsResponse> SendSmsAsync(string phone, string message, string? ip = null)
    {
        if (!string.IsNullOrEmpty(ip) && !_ipRegex.IsMatch(ip))
        {
            throw new InvalidMessageException("Provided field \"Ip\" is not valid");
        }
        var preparedMessage = PrepareMessage(message);
        var cost = await CheckCostAsync(phone, preparedMessage);
        if (cost > _options.MaxMessageCost)
        {
            throw new InvalidMessageException(
                $"Message cost ({cost}) is more than maximum cost ({_options.MaxMessageCost})");
        }
        var requestUrl = BaseUrl + $"sms/send?" +
                         $"api_id={_options.ApiId}&" +
                         $"to={phone}&" +
                         $"msg={preparedMessage}&" +
                         $"json=1" +
                         (_options.Test ? $"&test={_options.Test}" : "" )+
                         $"{(string.IsNullOrEmpty(_options.From) ? "" : $"&from={_options.From}")}" +
                         $"{(string.IsNullOrEmpty(ip) ? "" : $"&ip={ip}")}";
        var response = await _httpClient.GetAsync(requestUrl);
        var responseString = await response.Content.ReadAsStringAsync();
        var parsedResponse = JsonSerializer.Deserialize<BaseResponse<SmsResponse>>(responseString);
        if (parsedResponse is not { StatusCode: 100 } || !parsedResponse.Sms.Any())
            throw new SmsRuException("Failed to send code. Exception: " + parsedResponse!.Status);
        
        return parsedResponse.Sms.First().Value;

    }

    public async Task<double> GetBalanceAsync()
    {
        var requestUrl = BaseUrl + $"my/balance?api_id={_options.ApiId}&json=1";
        var response = await _httpClient.GetAsync(requestUrl);
        var responseString = await response.Content.ReadAsStringAsync();
        var parsedResponse = JsonSerializer.Deserialize<BalanceResponse>(responseString);
        if (parsedResponse is { Status: "OK" })
        {
            return parsedResponse.Balance;
        }
        throw new SmsRuException("Got unexpected response");
    }

    public async Task<string?> GetStatusAsync(string smsId)
    {
        var requestUrl = BaseUrl + $"sms/status?" +
            $"api_id={_options.ApiId}&" +
            $"sms_id={smsId}&" +
            $"json=1";
        var response = await _httpClient.GetAsync(requestUrl);
        var responseString = await response.Content.ReadAsStringAsync();
        var parsedResponse = JsonSerializer.Deserialize<BaseResponse<SmsRuBaseResponse>>(responseString);
        
        return parsedResponse switch
        {
            { StatusCode: 100 } => parsedResponse.Sms.First().Value.StatusText,
            { StatusCode: not 100 } => throw new SmsRuException("Failed to get status: " + parsedResponse.Status),
            _ => throw new SmsRuException("SmsRu send invalid response: " + responseString)
        };
    }

    public async Task<double> CheckCostAsync(string phone, string message)
    {
        var requestUrl = BaseUrl + "sms/cost?" +
                         $"api_id={_options.ApiId}&" +
                         $"to={phone}&" +
                         $"msg={PrepareMessage(message)}&" +
                         $"json=1" +
                         (string.IsNullOrEmpty(_options.From) ? "" : $"&from={_options.From}");
        var response = await _httpClient.GetAsync(requestUrl);
        var responseString = await response.Content.ReadAsStringAsync();
        var parsedResponse = JsonSerializer.Deserialize<CheckCostResponse>(responseString);
        if (parsedResponse is { StatusCode: 100 })
        {
            return parsedResponse.TotalCost;
        }

        throw new SmsRuException("Failed to check cost. Exception: " + parsedResponse!.Status);
    }

    public async Task<CallCodeResponse> SendCallCodeAsync(string phone, string? ip = null)
    {
        if (!string.IsNullOrEmpty(ip) && !_ipRegex.IsMatch(ip))
        {
            throw new InvalidMessageException("Provided field Ip is not valid");
        }
        var requestUrl = BaseUrl +
                         $"code/call?" +
                         $"api_id={_options.ApiId}&" +
                         $"phone={phone}" +
                         $"{(string.IsNullOrEmpty(ip) ? "" : $"&ip={ip}")}";
        var response = await _httpClient.GetAsync(requestUrl);
        var responseString = await response.Content.ReadAsStringAsync();
        try
        {
            var parsedResponse = JsonSerializer.Deserialize<CallCodeResponse>(responseString, _jsonSerializerOptions);
            if (response.StatusCode == HttpStatusCode.OK && parsedResponse is { Status: "OK", Code: not null })
            {
                return parsedResponse;
            }
            throw new SmsRuException("Failed to send code. Exception: " + parsedResponse?.StatusText);
        }
        catch (InvalidOperationException ex)
        {
            var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(responseString);
            if (parsed?.TryGetValue("status_text", out var statusText) ?? false)
            {
                throw new SmsRuException(statusText);
            }
            throw new JsonException("Exception has thrown while deserializing SmsRu response: " + ex.Message);
        }
    }

    public async Task<CheckLimitModel> GetLimitAsync()
    {
        var requestUrl = BaseUrl + $"my/limit?api_id={_options.ApiId}&json=1";
        var response = await _httpClient.GetAsync(requestUrl);
        var responseString = await response.Content.ReadAsStringAsync();
        var parsedResponse = JsonSerializer.Deserialize<CheckLimitModel>(responseString, _jsonSerializerOptions);
        if (parsedResponse is { Status: "OK" })
        {
            return parsedResponse;
        }
        throw new SmsRuException("Failed to send code. Exception: " + parsedResponse!.Status);
    }

    public async Task<bool> AuthIsValidAsync()
    {
        var requestUrl = BaseUrl + $"auth/check?api_id={_options.ApiId}&json=1";
        var response = await _httpClient.GetAsync(requestUrl);
        var responseString = await response.Content.ReadAsStringAsync();
        var parsedResponse = JsonSerializer.Deserialize<SmsRuBaseResponse>(responseString);
        return parsedResponse switch
        {
            { StatusCode: 100 } => true,
            { StatusCode: 200 } => false,
            _ => throw new SmsRuException("Failed check auth. Exception: " + parsedResponse!.StatusText)
        };
    }
    
    private string PrepareMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new InvalidMessageException($"Message is empty");
        }
        message = _anySpacesCharacterRegEx.Replace(message, "+");
        if (_anyRussianCharacterRegEx.IsMatch(message) &&
            Encoding.UTF8.GetBytes(message).Length > _options.MaxMessageLength)
        {
            message = IuliiaTranslator.Translate(message, Schemas.Mosmetro);
        }
        if (Encoding.UTF8.GetBytes(message).Length > _options.MaxMessageLength)
        {
            throw new InvalidMessageException($"Message length ({Encoding.UTF8.GetBytes(message).Length}) " +
                                              $"is more than max ({_options.MaxMessageLength}) length");
        }
        return message;
    }

    public string PreparePhone(string phone)
    {
        var preparedPhone = string.Concat(phone.Where(char.IsDigit));
        if (!_phoneRegEx.IsMatch(preparedPhone))
        {
            throw new ArgumentException("Phone is not valid");
        }
        return preparedPhone;
    }
}