namespace MyPetMonitor.Models;

public class Camera
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public int Port { get; set; } = 80;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<Profile> Profiles { get; set; } = new();
    public PTZCapabilities? PTZCapabilities { get; set; }
    public CameraStatus Status { get; set; } = CameraStatus.Offline;
    public string? CurrentStreamUrl { get; set; }
    public DateTime LastSeen { get; set; } = DateTime.UtcNow;
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? FirmwareVersion { get; set; }
}

public class Profile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string StreamUri { get; set; } = string.Empty;
    public VideoResolution Resolution { get; set; } = new();
    public int FrameRate { get; set; } = 30;
    public string Encoding { get; set; } = "H264";
    public int Quality { get; set; } = 5;
}

public class VideoResolution
{
    public int Width { get; set; } = 1920;
    public int Height { get; set; } = 1080;
    
    public override string ToString() => $"{Width}x{Height}";
}

public class PTZCapabilities
{
    public bool HasPTZ { get; set; }
    public bool CanPan { get; set; }
    public bool CanTilt { get; set; }
    public bool CanZoom { get; set; }
    public bool HasPresets { get; set; }
    public List<PTZPreset> Presets { get; set; } = new();
}

public class PTZPreset
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public PTZPosition Position { get; set; } = new();
}

public class PTZPosition
{
    public float Pan { get; set; }
    public float Tilt { get; set; }
    public float Zoom { get; set; }
}

public class PTZCommand
{
    public string CameraId { get; set; } = string.Empty;
    public PTZVector Pan { get; set; } = new();
    public PTZVector Tilt { get; set; } = new();
    public PTZVector Zoom { get; set; } = new();
    public float Speed { get; set; } = 0.5f;
}

public class PTZVector
{
    public float X { get; set; }
    public float Y { get; set; }
}

public enum CameraStatus
{
    Offline,
    Online,
    Connecting,
    Streaming,
    Error
}
