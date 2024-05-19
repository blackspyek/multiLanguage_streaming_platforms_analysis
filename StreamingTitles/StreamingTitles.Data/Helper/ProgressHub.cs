using Microsoft.AspNetCore.SignalR;

public class ProgressHub : Hub
{
    public async Task SendProgressUpdate(int remainingRows)
    {
        await Clients.All.SendAsync("ProgressUpdate", remainingRows);
    }
}