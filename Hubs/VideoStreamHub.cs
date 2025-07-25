using Microsoft.AspNetCore.SignalR;
using MyPetMonitor.Services.Interfaces;

namespace MyPetMonitor.Hubs;

/// <summary>
/// Hub de SignalR para streaming de video en tiempo real
/// </summary>
public class VideoStreamHub : Hub
{
    private readonly ILogger<VideoStreamHub> _logger;
    private readonly ICameraService _cameraService;

    public VideoStreamHub(ILogger<VideoStreamHub> logger, ICameraService cameraService)
    {
        _logger = logger;
        _cameraService = cameraService;
    }

    public async Task JoinCameraGroup(string cameraId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Camera_{cameraId}");
        _logger.LogInformation("Client {ConnectionId} joined camera group {CameraId}", Context.ConnectionId, cameraId);
    }

    public async Task LeaveCameraGroup(string cameraId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Camera_{cameraId}");
        _logger.LogInformation("Client {ConnectionId} left camera group {CameraId}", Context.ConnectionId, cameraId);
    }

    public async Task StartStream(string cameraId)
    {
        try
        {
            var success = await _cameraService.StartStreamAsync(cameraId);
            if (success)
            {
                await Clients.Group($"Camera_{cameraId}").SendAsync("StreamStarted", cameraId);
                _logger.LogInformation("Stream started for camera {CameraId}", cameraId);
            }
            else
            {
                await Clients.Caller.SendAsync("StreamError", cameraId, "Failed to start stream");
                _logger.LogWarning("Failed to start stream for camera {CameraId}", cameraId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting stream for camera {CameraId}", cameraId);
            await Clients.Caller.SendAsync("StreamError", cameraId, ex.Message);
        }
    }

    public async Task StopStream(string cameraId)
    {
        try
        {
            var success = await _cameraService.StopStreamAsync(cameraId);
            if (success)
            {
                await Clients.Group($"Camera_{cameraId}").SendAsync("StreamStopped", cameraId);
                _logger.LogInformation("Stream stopped for camera {CameraId}", cameraId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping stream for camera {CameraId}", cameraId);
        }
    }

    public async Task SendFrame(string cameraId, byte[] frameData)
    {
        try
        {
            await Clients.Group($"Camera_{cameraId}").SendAsync("FrameReceived", cameraId, frameData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending frame for camera {CameraId}", cameraId);
        }
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client {ConnectionId} connected to VideoStreamHub", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client {ConnectionId} disconnected from VideoStreamHub", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
