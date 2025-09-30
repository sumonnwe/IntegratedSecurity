// BiometricReaderAdapter : IBiometricReader with in-memory templates for demo.
// Enroll userId->template; Verify compares sample vs stored template.
using System.Collections.Concurrent;
using IntegratedSecurity.Domain.Devices;

namespace IntegratedSecurity.Infrastructure.Devices;

public sealed class BiometricReaderAdapter : IBiometricReader
{
    private readonly ConcurrentDictionary<string, byte[]> _store = new();
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; } = "Biometric-Adapter";

    public Task<bool> EnrollAsync(string userId, byte[] template, CancellationToken ct)
        => Task.FromResult(_store.TryAdd(userId, template));

    public Task<bool> VerifyAsync(string userId, byte[] sample, CancellationToken ct)
        => Task.FromResult(_store.TryGetValue(userId, out var t) && t.SequenceEqual(sample));
}
