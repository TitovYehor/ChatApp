using Microsoft.AspNetCore.SignalR.Client;

Console.WriteLine("JWT Token:");

var token = Console.ReadLine();

var connection = new HubConnectionBuilder()
    .WithUrl(
        "https://localhost:7163/hubs/chat",
        options =>
        {
            options.AccessTokenProvider =
                () => Task.FromResult(token);
        })
    .WithAutomaticReconnect()
    .Build();

try
{
    await connection.StartAsync();

    Console.WriteLine($"Connected. ConnectionId: {connection.ConnectionId}");
}
catch (Exception ex)
{
    Console.WriteLine($"Connection failed: {ex.Message}");
}

Console.WriteLine("Press Enter to exit");
Console.ReadLine();