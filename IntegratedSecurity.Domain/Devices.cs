// DDD device abstractions for Integrated Security Management System:
// IDevice, IAccessController (card, doors, lifts, fire),
// ICctvCamera (ONVIF+RTSP, PTZ), IBiometricReader (enroll/verify).
// Domain events: CardScannedEvent, DoorOpenedEvent, LiftAccessGrantedEvent, FireAlarmTriggeredEvent, BiometricVerifiedEvent.
// Use immutable records with OccurredAt.
namespace IntegratedSecurity.Domain.Devices;

public interface IDevice { Guid Id { get; } string Name { get; } }

public interface IAccessController : IDevice
{
    Task<bool> GrantDoorAccessAsync(string doorId, string cardNo, CancellationToken ct);
    Task<bool> ControlLiftAsync(string floor, string cardNo, CancellationToken ct);
    Task TriggerFireAlarmAsync(CancellationToken ct);
}

public interface ICctvCamera : IDevice
{
    Uri OnvifEndpoint { get; }
    Uri RtspUri { get; }
    Task<bool> PTZAsync(float pan, float tilt, float zoom, CancellationToken ct);
}

public interface IBiometricReader : IDevice
{
    Task<bool> EnrollAsync(string userId, byte[] template, CancellationToken ct);
    Task<bool> VerifyAsync(string userId, byte[] sample, CancellationToken ct);
}

public abstract record DomainEvent(DateTimeOffset OccurredAt);
public record CardScannedEvent(string CardNo, string ReaderId, DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
public record DoorOpenedEvent(string DoorId, string Reason, DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
public record LiftAccessGrantedEvent(string Floor, string CardNo, DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
public record FireAlarmTriggeredEvent(string Zone, DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
public record BiometricVerifiedEvent(string UserId, bool Success, DateTimeOffset OccurredAt) : DomainEvent(OccurredAt);
