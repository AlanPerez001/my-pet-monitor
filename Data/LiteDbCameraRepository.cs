using LiteDB;
using MyPetMonitor.Models;
using MyPetMonitor.Services.Interfaces;

namespace MyPetMonitor.Data;

public class LiteDbCameraRepository : ICameraRepository, IDisposable
{
    private readonly LiteDatabase _database;
    private readonly ILiteCollection<Camera> _cameras;
    private readonly ILogger<LiteDbCameraRepository> _logger;

    public LiteDbCameraRepository(ILogger<LiteDbCameraRepository> logger)
    {
        _logger = logger;
        
        // Crear directorio de datos si no existe
        var dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyPetMonitor");
        Directory.CreateDirectory(dataPath);
        
        var dbPath = Path.Combine(dataPath, "cameras.db");
        _database = new LiteDatabase(dbPath);
        _cameras = _database.GetCollection<Camera>("cameras");
        
        // Crear índices
        _cameras.EnsureIndex(x => x.Id);
        _cameras.EnsureIndex(x => x.IpAddress);
        
        _logger.LogInformation("LiteDB repository initialized at {DbPath}", dbPath);
    }

    public async Task<IEnumerable<Camera>> GetAllAsync()
    {
        try
        {
            await Task.Delay(0); // Para mantener la interfaz async
            var cameras = _cameras.FindAll().ToList();
            _logger.LogDebug("Retrieved {Count} cameras from database", cameras.Count);
            return cameras;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all cameras");
            return Enumerable.Empty<Camera>();
        }
    }

    public async Task<Camera?> GetByIdAsync(string id)
    {
        try
        {
            await Task.Delay(0);
            var camera = _cameras.FindById(id);
            _logger.LogDebug("Retrieved camera {CameraId}: {Found}", id, camera != null ? "Found" : "Not Found");
            return camera;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving camera {CameraId}", id);
            return null;
        }
    }

    public async Task<Camera> AddAsync(Camera camera)
    {
        try
        {
            await Task.Delay(0);
            
            if (string.IsNullOrEmpty(camera.Id))
            {
                camera.Id = Guid.NewGuid().ToString();
            }
            
            camera.LastSeen = DateTime.UtcNow;
            _cameras.Insert(camera);
            _logger.LogInformation("Added camera {CameraId} ({CameraName})", camera.Id, camera.Name);
            return camera;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding camera {CameraName}", camera.Name);
            throw;
        }
    }

    public async Task<Camera> UpdateAsync(Camera camera)
    {
        try
        {
            await Task.Delay(0);
            camera.LastSeen = DateTime.UtcNow;
            var updated = _cameras.Update(camera);
            
            if (updated)
            {
                _logger.LogInformation("Updated camera {CameraId} ({CameraName})", camera.Id, camera.Name);
            }
            else
            {
                _logger.LogWarning("Camera {CameraId} not found for update", camera.Id);
            }
            
            return camera;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating camera {CameraId}", camera.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            await Task.Delay(0);
            var deleted = _cameras.Delete(id);
            
            if (deleted)
            {
                _logger.LogInformation("Deleted camera {CameraId}", id);
            }
            else
            {
                _logger.LogWarning("Camera {CameraId} not found for deletion", id);
            }
            
            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting camera {CameraId}", id);
            return false;
        }
    }

    public async Task<Camera?> GetByIpAddressAsync(string ipAddress)
    {
        try
        {
            await Task.Delay(0);
            var camera = _cameras.FindOne(x => x.IpAddress == ipAddress);
            _logger.LogDebug("Retrieved camera by IP {IpAddress}: {Found}", ipAddress, camera != null ? "Found" : "Not Found");
            return camera;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving camera by IP {IpAddress}", ipAddress);
            return null;
        }
    }

    public void Dispose()
    {
        _database?.Dispose();
        _logger.LogInformation("LiteDB repository disposed");
    }
}
