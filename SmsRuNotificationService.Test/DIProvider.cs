using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using SmsRuNotificationService.Configuration;

namespace SmsRuNotificationService.Test;

public static class DIProvider
{
    public static ServiceProvider ServiceProvider { get; set; }
    
    static DIProvider()
    {
        using var appSettingsValue = new StreamReader("appsettings.Test.json");
        var appSettingsJson = appSettingsValue.ReadToEnd();
        var settings = JsonSerializer.Deserialize<SmsRuSenderOptions>(appSettingsJson);

        if (settings is null)
        {
            throw new NullReferenceException("SmsRu settings could not be got from appsettings file");
        }

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSmsRuClient(options =>
        {
            options.Test = settings.Test;
            options.MaxMessageCost = settings.MaxMessageCost;
            options.From = settings.From;
            options.MaxMessageLength = settings.MaxMessageLength;
            options.ApiId = settings.ApiId;
        });
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
}