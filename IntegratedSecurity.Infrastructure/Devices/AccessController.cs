// Implement AccessControllerAdapter : IAccessController.
// For now, simulate device I/O; later replace with vendor SDK.
// Methods: GrantDoorAccessAsync, ControlLiftAsync, TriggerFireAlarmAsync with logging and Task.Delay simulation.
using IntegratedSecurity.Domain.Devices;
using Microsoft.Extensions.Logging;

namespace IntegratedSecurity.Infrastructure.Devices;

public sealed class AccessControllerAdapter(ILogger<AccessControllerAdapter> logger) : IAccessController
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; } = "SEMAC-Adapter";

    public async Task<bool> GrantDoorAccessAsync(string doorId, string cardNo, CancellationToken ct)
    {
        logger.LogInformation("Granting door access: {Door} for card {Card}", doorId, cardNo);
        await Task.Delay(50, ct); return true;
    }

    public async Task<bool> ControlLiftAsync(string floor, string cardNo, CancellationToken ct)
    {
        logger.LogInformation("Calling lift to floor {Floor} for card {Card}", floor, cardNo);
        await Task.Delay(50, ct); return true;
    }

    public Task TriggerFireAlarmAsync(CancellationToken ct)
    {
        logger.LogWarning("FIRE ALARM TRIGGERED!");
        return Task.CompletedTask;
    }
}
