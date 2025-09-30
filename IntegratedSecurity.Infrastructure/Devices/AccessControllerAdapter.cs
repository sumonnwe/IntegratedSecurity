// AccessControllerAdapter : IAccessController for doors/lifts/fire.
// Simulate vendor SDK I/O with logging and Task.Delay; later swap with real SDK.
using IntegratedSecurity.Domain.Devices;
using Microsoft.Extensions.Logging;

namespace IntegratedSecurity.Infrastructure.Devices;

public sealed class AccessControllerAdapter(ILogger<AccessControllerAdapter> log) : IAccessController
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; } = "SEMAC-Adapter";

    public async Task<bool> GrantDoorAccessAsync(string doorId, string cardNo, CancellationToken ct)
    { log.LogInformation("Door {Door} opened for {Card}", doorId, cardNo); await Task.Delay(30, ct); return true; }

    public async Task<bool> ControlLiftAsync(string floor, string cardNo, CancellationToken ct)
    { log.LogInformation("Lift access floor {Floor} for {Card}", floor, cardNo); await Task.Delay(30, ct); return true; }

    public Task TriggerFireAlarmAsync(CancellationToken ct)
    { log.LogWarning("FIRE ALARM TRIGGERED"); return Task.CompletedTask; }
}
