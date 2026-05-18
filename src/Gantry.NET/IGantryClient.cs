namespace Gantry.NET;

public interface IGantryClient
{
    Task<bool> Ping(CancellationToken ct);
    Task Put(string message, CancellationToken ct);
    Task<string> GetAsString(int offset, CancellationToken ct);
    Task<byte[]> Get(int offset, CancellationToken ct);
}
