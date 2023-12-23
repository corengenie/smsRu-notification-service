using Microsoft.Extensions.DependencyInjection;
using SmsRuNotificationService.Core;

namespace SmsRuNotificationService.Test;

public class SmsRuClientTest
{
    private readonly ISmsSender _sut;

    public SmsRuClientTest()
    {
        _sut = DIProvider.ServiceProvider.GetRequiredService<ISmsSender>();
    }

    [Test]
    public async Task AuthIsValid()
    {
        var authIsValid = await _sut.AuthIsValidAsync();
        Assert.That(authIsValid, Is.True);
    }
}