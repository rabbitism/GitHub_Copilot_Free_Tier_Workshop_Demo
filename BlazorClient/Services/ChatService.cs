using BlazorClient.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorClient.Services;

public class ChatService
{
    private HubConnection _connection;
    public List<ChatMessage> Messages { get; } = new();

    public event Action OnMessageReceived;

    public ChatService()
    {
        _connection = new HubConnectionBuilder()
                      .WithUrl("http://localhost:5032/chathub") // Replace with your hub URL
                      .Build();

        _connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            Messages.Add(new ChatMessage { User = user, Message = message });
            OnMessageReceived?.Invoke();
        });
    }

    public async Task ConnectAsync()
    {
        try
        {
            await _connection.StartAsync();
            Messages.Add(new ChatMessage { User = "System", Message = "Connected to chat hub." });
            OnMessageReceived?.Invoke();
        }
        catch (Exception ex)
        {
            Messages.Add(new ChatMessage { User = "System", Message = $"Error connecting to chat hub: {ex.Message}" });
            OnMessageReceived?.Invoke();
        }
    }

    public async Task SendMessageAsync(string user, string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            await _connection.InvokeAsync("SendMessage", user, message);
        }
    }
}