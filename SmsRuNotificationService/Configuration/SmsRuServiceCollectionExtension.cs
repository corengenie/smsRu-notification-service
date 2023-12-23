using Microsoft.Extensions.DependencyInjection;
using SmsRuNotificationService.Core;
using SmsRuNotificationService.Exceptions;

namespace SmsRuNotificationService.Configuration;

public static class SmsRuServiceCollectionExtension
{
    public static IServiceCollection AddSmsRuClient(
        this IServiceCollection serviceCollection, Action<SmsRuSenderOptions> options)
    {
        if (serviceCollection.Any(d => d.ServiceType == typeof(ISmsSender)))
        {
            throw new SmsRuClientConfigurationException(
                $"{nameof(AddSmsRuClient)}() was already called and may only be called once per container.");
        }
        
        serviceCollection.AddSingleton<ISmsSender>(_ => new SmsSender(options));
        return serviceCollection;
    }
}