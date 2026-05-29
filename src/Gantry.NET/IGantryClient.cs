namespace Gantry.NET;

public interface IGantryClient
{
    Task<bool> Ping(CancellationToken ct);
    Task Put(string message, int topicId, CancellationToken ct);
    Task<string> GetAsString(int offset, int topicId, CancellationToken ct);
    Task<byte[]> Get(int offset, int topicId, CancellationToken ct);
}
