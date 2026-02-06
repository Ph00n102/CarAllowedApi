
// ในโปรเจค API (Server)
using CarAllowedApi.Dto;
using Microsoft.AspNetCore.SignalR;

namespace CarAllowedApi.Hubs;
public class JobDistributeHub : Hub
{
    // Methods ที่ต้องมีให้ตรงกับ Client
    public async Task SendJobRequestUpdate(string message)
    {
        await Clients.All.SendAsync("ReceiveJobRequestUpdate", message);
    }

    public async Task SendUpdatedJobRequest(JobRequestCarDto dto)
    {
        await Clients.All.SendAsync("ReceiveUpdatedJobRequest", dto);
    }

    public async Task SendDeletedJobRequest(int id)
    {
        await Clients.All.SendAsync("ReceiveDeletedJobRequest", id);
    }

    // Optional: เพิ่ม method สำหรับการทดสอบ
    public async Task TestConnection(string message)
    {
        await Clients.Caller.SendAsync("ReceiveTestMessage", $"Server received: {message}");
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        
        // แจ้งเตือน client อื่นๆ
        await Clients.Others.SendAsync("ReceiveNotification", "System", $"New client connected: {Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
    }
}