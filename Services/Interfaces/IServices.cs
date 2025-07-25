using MyPetMonitor.Models;

namespace MyPetMonitor.Services.Interfaces;

public interface ICameraService
{
    Task<IEnumerable<Camera>> GetCamerasAsync();
    Task<Camera?> GetCameraByIdAsync(string id);
    Task<Camera> AddCameraAsync(Camera camera);
    Task<Camera> UpdateCameraAsync(Camera camera);
    Task<bool> DeleteCameraAsync(string id);
    Task<IEnumerable<Camera>> DiscoverCamerasAsync();
    Task<bool> TestConnectionAsync(string cameraId);
    Task<IEnumerable<Profile>> GetProfilesAsync(string cameraId);
    Task<bool> StartStreamAsync(string cameraId, string? profileId = null);
    Task<bool> StopStreamAsync(string cameraId);
}

public interface IOnvifService
{
    Task<Camera?> ConnectCameraAsync(string ipAddress, string username, string password);
    Task<IEnumerable<Profile>> GetMediaProfilesAsync(Camera camera);
    Task<PTZCapabilities?> GetPTZCapabilitiesAsync(Camera camera);
    Task<bool> TestConnectionAsync(Camera camera);
    Task<IEnumerable<Camera>> DiscoverDevicesAsync(TimeSpan timeout);
}

public interface IStreamingService
{
    Task<Stream?> GetVideoStreamAsync(string cameraId, string profileId);
    Task<bool> StartStreamingAsync(string cameraId, string profileId);
    Task<bool> StopStreamingAsync(string cameraId);
    Task<string?> GetStreamUrlAsync(string cameraId, string profileId);
    event EventHandler<FrameReceivedEventArgs>? FrameReceived;
}

public interface IPTZService
{
    Task<bool> ExecutePTZCommandAsync(PTZCommand command);
    Task<PTZPosition?> GetCurrentPositionAsync(string cameraId);
    Task<bool> MoveToPresentAsync(string cameraId, string presetId);
    Task<bool> SetPresetAsync(string cameraId, string presetName);
    Task<IEnumerable<PTZPreset>> GetPresetsAsync(string cameraId);
}

public interface ICameraRepository
{
    Task<IEnumerable<Camera>> GetAllAsync();
    Task<Camera?> GetByIdAsync(string id);
    Task<Camera> AddAsync(Camera camera);
    Task<Camera> UpdateAsync(Camera camera);
    Task<bool> DeleteAsync(string id);
    Task<Camera?> GetByIpAddressAsync(string ipAddress);
}

// Event Args
public class FrameReceivedEventArgs : EventArgs
{
    public string CameraId { get; set; } = string.Empty;
    public byte[] FrameData { get; set; } = Array.Empty<byte>();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
