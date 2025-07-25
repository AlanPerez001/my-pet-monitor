using MyPetMonitor.Models;
using MyPetMonitor.Services.Interfaces;

namespace MyPetMonitor.Services;

public class CameraService : ICameraService
{
    private readonly ICameraRepository _repository;
    private readonly IOnvifService _onvifService;
    private readonly ILogger<CameraService> _logger;

    public CameraService(
        ICameraRepository repository,
        IOnvifService onvifService,
        ILogger<CameraService> logger)
    {
        _repository = repository;
        _onvifService = onvifService;
        _logger = logger;
    }

    public async Task<IEnumerable<Camera>> GetCamerasAsync()
    {
        try
        {
            return await _repository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cameras");
            return Enumerable.Empty<Camera>();
        }
    }

    public async Task<Camera?> GetCameraByIdAsync(string id)
    {
        try
        {
            return await _repository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting camera {CameraId}", id);
            return null;
        }
    }

    public async Task<Camera> AddCameraAsync(Camera camera)
    {
        try
        {
            // Validar conexión antes de agregar
            var isConnected = await _onvifService.TestConnectionAsync(camera);
            if (!isConnected)
            {
                camera.Status = CameraStatus.Error;
                _logger.LogWarning("Camera {CameraName} failed connection test", camera.Name);
            }
            else
            {
                camera.Status = CameraStatus.Online;
                
                // Obtener perfiles si está conectada
                var profiles = await _onvifService.GetMediaProfilesAsync(camera);
                camera.Profiles = profiles.ToList();
                
                // Obtener capacidades PTZ
                camera.PTZCapabilities = await _onvifService.GetPTZCapabilitiesAsync(camera);
            }

            return await _repository.AddAsync(camera);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding camera {CameraName}", camera.Name);
            throw;
        }
    }

    public async Task<Camera> UpdateCameraAsync(Camera camera)
    {
        try
        {
            return await _repository.UpdateAsync(camera);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating camera {CameraId}", camera.Id);
            throw;
        }
    }

    public async Task<bool> DeleteCameraAsync(string id)
    {
        try
        {
            return await _repository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting camera {CameraId}", id);
            return false;
        }
    }

    public async Task<IEnumerable<Camera>> DiscoverCamerasAsync()
    {
        try
        {
            _logger.LogInformation("Starting camera discovery...");
            var discoveredCameras = await _onvifService.DiscoverDevicesAsync(TimeSpan.FromSeconds(10));
            
            _logger.LogInformation("Discovered {Count} cameras", discoveredCameras.Count());
            return discoveredCameras;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during camera discovery");
            return Enumerable.Empty<Camera>();
        }
    }

    public async Task<bool> TestConnectionAsync(string cameraId)
    {
        try
        {
            var camera = await _repository.GetByIdAsync(cameraId);
            if (camera == null)
            {
                return false;
            }

            return await _onvifService.TestConnectionAsync(camera);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing connection for camera {CameraId}", cameraId);
            return false;
        }
    }

    public async Task<IEnumerable<Profile>> GetProfilesAsync(string cameraId)
    {
        try
        {
            var camera = await _repository.GetByIdAsync(cameraId);
            if (camera == null)
            {
                return Enumerable.Empty<Profile>();
            }

            return await _onvifService.GetMediaProfilesAsync(camera);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profiles for camera {CameraId}", cameraId);
            return Enumerable.Empty<Profile>();
        }
    }

    public async Task<bool> StartStreamAsync(string cameraId, string? profileId = null)
    {
        try
        {
            var camera = await _repository.GetByIdAsync(cameraId);
            if (camera == null)
            {
                return false;
            }

            // TODO: Implementar inicio de streaming
            camera.Status = CameraStatus.Streaming;
            await _repository.UpdateAsync(camera);
            
            _logger.LogInformation("Started streaming for camera {CameraId}", cameraId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting stream for camera {CameraId}", cameraId);
            return false;
        }
    }

    public async Task<bool> StopStreamAsync(string cameraId)
    {
        try
        {
            var camera = await _repository.GetByIdAsync(cameraId);
            if (camera == null)
            {
                return false;
            }

            // TODO: Implementar detención de streaming
            camera.Status = CameraStatus.Online;
            camera.CurrentStreamUrl = null;
            await _repository.UpdateAsync(camera);
            
            _logger.LogInformation("Stopped streaming for camera {CameraId}", cameraId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping stream for camera {CameraId}", cameraId);
            return false;
        }
    }
}
