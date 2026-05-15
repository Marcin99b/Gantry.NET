using System.Runtime.CompilerServices;

namespace Gantry.NET;

public interface IGantryClient
{
    IAsyncEnumerable<string> Iterate(int fromOffset, CancellationToken ct);
    Task Put(string message, CancellationToken ct);
    Task<string> Get(int offset, CancellationToken ct);
    Task<int> MaxOffset(CancellationToken ct);
}
