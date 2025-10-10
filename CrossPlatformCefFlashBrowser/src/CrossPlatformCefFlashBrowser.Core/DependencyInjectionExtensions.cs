using Microsoft.Extensions.DependencyInjection;

namespace CrossPlatformCefFlashBrowser.Core;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // 后续将在此注册跨平台通用的服务与 ViewModel。
        return services;
    }
}
