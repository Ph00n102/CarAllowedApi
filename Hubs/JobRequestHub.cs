using CarAllowedApi.Dto;
using Microsoft.AspNetCore.SignalR;

namespace CarAllowedApi.Hubs
{
     public class JobRequestHub : Hub
    {
        // 1. ส่งข้อมูลใหม่
        public async Task SendNewJobRequest(JobRequestCarDto jobRequest)
        {
            await Clients.All.SendAsync("ReceiveNewJobRequest", jobRequest);
        }

        // 2. ส่งข้อมูลที่อัพเดท
        public async Task SendUpdatedJobRequest(JobRequestCarDto jobRequest)
        {
            await Clients.All.SendAsync("ReceiveUpdatedJobRequest", jobRequest);
        }

        // 3. ส่งข้อมูลที่ถูกลบ
        public async Task SendDeletedJobRequest(int id)
        {
            await Clients.All.SendAsync("ReceiveDeletedJobRequest", id);
        }

        // 4. ส่งข้อความอัพเดททั่วไป
        public async Task SendJobRequestUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveJobRequestUpdate", message);
        }

        // 5. ตรวจสอบการเชื่อมต่อ
        public async Task TestConnection(string message)
        {
            await Clients.Caller.SendAsync("ReceiveTestMessage", $"Server received: {message}");
        }

        // เมื่อ client เชื่อมต่อ
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        // เมื่อ client ตัดการเชื่อมต่อ
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
        
    }
}