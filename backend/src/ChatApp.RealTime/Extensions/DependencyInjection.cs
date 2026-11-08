using ChatApp.Application.Interfaces;
using ChatApp.RealTime.Notifications;
using ChatApp.RealTime.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.RealTime.Extensions;

/// <summary>
/// Provides dependency injection registration for realtime functionality
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers SignalR services and the realtime implementation services
    /// </summary>
    public static IServiceCollection AddRealTime(
        this IServiceCollection services)
    {
        services.AddSignalR();

        services.AddSingleton<IUserIdProvider, UserIdProvider>();

        services.AddScoped<IChatNotifier, SignalRChatNotifier>();

        services.AddSingleton<IOnlineUserTracker, OnlineUserTracker>();

        services.AddScoped<IPresenceService, PresenceService>();

        services.AddScoped<IWorkspaceNotifier, SignalRWorkspaceNotifier>();

        services.AddScoped<IChannelNotifier, SignalRChannelNotifier>();

        services.AddScoped<INotificationNotifier, SignalRNotificationNotifier>();

        services.AddScoped<IFriendshipNotifier, SignalRFriendshipNotifier>();

        return services;
    }
}