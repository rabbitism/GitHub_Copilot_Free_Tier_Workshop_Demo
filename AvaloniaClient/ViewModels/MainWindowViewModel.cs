using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AvaloniaClient.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;

namespace AvaloniaClient.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private HubConnection _connection;
    
    public IAsyncRelayCommand ConnectCommand { get; }
    public IAsyncRelayCommand SendMessageCommand { get; }
    
    public ObservableCollection<ChatMessage> Messages { get; } = new();
    
    [ObservableProperty]
    private string _message;

    public MainWindowViewModel()
    {
        _connection = new HubConnectionBuilder()
                      .WithUrl("http://localhost:5032/chathub") // Replace with your hub URL
                      .Build();
        _connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            Messages.Add(new ChatMessage { User = user, Message = message });
        });
        _connection.StartAsync();
        ConnectCommand = new AsyncRelayCommand(ConnectAsync);
        SendMessageCommand = new AsyncRelayCommand(SendMessageAsync);
    }
    

    private async Task ConnectAsync()
    {
        try
        {
            await _connection.StartAsync();
            Messages.Add(new ChatMessage { User = "System", Message = "Connected to chat hub." });
        }
        catch (Exception ex)
        {
            Messages.Add(new ChatMessage { User = "System", Message = $"Error connecting to chat hub: {ex.Message}" });
        }
    }

    private async Task SendMessageAsync()
    {
        if (!string.IsNullOrEmpty(Message))
        {
            await _connection.InvokeAsync("SendMessage", "AvaloniaClient", Message);
            Message = string.Empty;
        }
    }
}


