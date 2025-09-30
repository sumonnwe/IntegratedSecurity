// Implement CardScanHandler that validates card, simulates authorization (cards starting with 'A'),
// publishes CardScanned + LiftAccessGranted via IEventBus.
// Keep business logic here (Application).

using IntegratedSecurity.Application.Abstractions;

namespace IntegratedSecurity.Application.UseCases;

public record CardScanned(string CardNo, string ReaderId) : IEvent;
public record LiftAccessGranted(string ReaderId, string CardNo) : IEvent;

public sealed class CardScanHandler(IEventBus bus)
{
    public async Task<bool> HandleAsync(string cardNo, string readerId, CancellationToken ct)
    {
        var ok = cardNo.StartsWith("A", StringComparison.OrdinalIgnoreCase);
        await bus.PublishAsync(new CardScanned(cardNo, readerId), ct);
        if (ok) await bus.PublishAsync(new LiftAccessGranted(readerId, cardNo), ct);
        return ok;
    }
}
