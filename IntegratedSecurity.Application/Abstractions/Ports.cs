// Event-driven ports:
// IEvent marker, IEventBus (PublishAsync<T>, Subscribe<T>).
// IAccessControlService for card-scan -> authorization -> publish events.
namespace IntegratedSecurity.Application.Abstractions;

public interface IEvent { }

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent evt, CancellationToken ct = default) where TEvent : IEvent;
    void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler) where TEvent : IEvent;
}

public interface IAccessControlService
{
    Task<bool> HandleCardScanAsync(string cardNo, string readerId, CancellationToken ct);
}
