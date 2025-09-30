// OnvifCctvCamera : ICctvCamera. Expose OnvifEndpoint + RtspUri; PTZAsync uses IOnvifClient abstraction (swappable).
using IntegratedSecurity.Domain.Devices;

namespace IntegratedSecurity.Infrastructure.Devices;

public interface IOnvifClient { Task<bool> PTZAsync(Uri endpoint, float pan, float tilt, float zoom, CancellationToken ct); }

public sealed class OnvifCctvCamera(string name, Uri onvif, Uri rtsp, IOnvifClient client) : ICctvCamera
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; } = name;
    public Uri OnvifEndpoint { get; } = onvif;
    public Uri RtspUri { get; } = rtsp;
    public Task<bool> PTZAsync(float pan, float tilt, float zoom, CancellationToken ct) => client.PTZAsync(OnvifEndpoint, pan, tilt, zoom, ct);
}
