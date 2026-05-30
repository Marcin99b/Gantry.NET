namespace Gantry.NET;

public interface IGantryClient
{
    Task<bool> Ping(CancellationToken ct);
    Task Put(uint topicId, string message, CancellationToken ct);
    Task<string> GetAsString(uint topicId, uint offset,  CancellationToken ct);
    Task<byte[]> Get(uint topicId, uint offset, CancellationToken ct);
}
