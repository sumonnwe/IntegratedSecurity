// BackgroundService: Subscribe to CardScanned and LiftAccessGranted via IEventBus; log results.
// Later: call AccessControllerAdapter to open doors or command lifts.
using IntegratedSecurity.Application.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IntegratedSecurity.Worker;

public sealed class EventWorker(IEventBus bus, ILogger<EventWorker> log) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bus.Subscribe<CardScanned>(async (e, _) => { log.LogInformation("Card {Card} @ {Reader}", e.CardNo, e.ReaderId); await Task.CompletedTask; });
        bus.Subscribe<LiftAccessGranted>(async (e, _) => { log.LogInformation("Lift granted for {Card} via {Reader}", e.CardNo, e.ReaderId); await Task.CompletedTask; });
        return Task.CompletedTask;
    }
}

public record CardScanned(string CardNo, string ReaderId) : IEvent;
public record LiftAccessGranted(string ReaderId, string CardNo) : IEvent;
