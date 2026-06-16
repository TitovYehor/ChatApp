using ChatApp.RealTime.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.RealTime.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddRealTime(
        this IServiceCollection services)
    {
        services.AddSignalR();

        services.AddSingleton<IUserIdProvider, UserIdProvider>();

        return services;
    }
}