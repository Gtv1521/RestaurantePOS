using System;
using System.Threading.Tasks;
using MiComanderaApp.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace MiComanderaApp.Core.Infrastructure.SignalR;

public class SignalRService
{
    private readonly HubConnection _connection;

    public event Action? Connected;
    public event Action? Reconnecting;
    public event Action? Disconnected;

    public SignalRService(IOptions<ApiSettings> options)
    {
        var url = $"{options.Value.SignalR}";

        _connection = new HubConnectionBuilder()
            .WithUrl(url)
            .WithAutomaticReconnect()
            .Build();


        _connection.Reconnecting += error =>
        {
            Reconnecting?.Invoke();
            return Task.CompletedTask;
        };

        _connection.Reconnected += connectionId =>
        {
            Connected?.Invoke();
            return Task.CompletedTask;
        };

        _connection.Closed += error =>
        {
            Disconnected?.Invoke();
            return Task.CompletedTask;
        };
    }


    public HubConnection Connection => _connection;


    public bool IsConnected =>
        _connection.State == HubConnectionState.Connected;


    public async Task ConnectAsync()
    {
        if (!IsConnected)
        {
            await _connection.StartAsync();
            Connected?.Invoke();
        }
    }


    public async Task DisconnectAsync()
    {
        await _connection.StopAsync();
        await _connection.DisposeAsync();
    }
}