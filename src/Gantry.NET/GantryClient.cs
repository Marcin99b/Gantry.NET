using System.Net.Sockets;
using System.Text;

namespace Gantry.NET;

internal class GantryClient(GantryOptions options) : IGantryClient
{
    private static readonly IReadOnlyDictionary<CommandType, ReadOnlyMemory<byte>> commandTypeBytes = Enum.GetValues<CommandType>().ToDictionary(x => x, x => new ReadOnlyMemory<byte>([(byte)x]));

    public async Task<bool> Ping(CancellationToken ct)
        => (await this.Send(CommandType.Ping, ct)).SingleOrDefault() == 1;

    public Task Put(string message, CancellationToken ct) 
        => this.Send(CommandType.PutMessage, Encoding.UTF8.GetBytes(message), ct);

    public async Task<string> GetAsString(int offset, CancellationToken ct)
        => Encoding.UTF8.GetString(await this.Get(offset, ct));

    public Task<byte[]> Get(int offset, CancellationToken ct)
        => this.Send(CommandType.GetMessage, BitConverter.GetBytes(offset), ct);

    private async Task<byte[]> Send(CommandType commandType, ReadOnlyMemory<byte> data, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var address = options.GetAddress();
        using var client = new TcpClient(address.Host, address.Port);
        using var stream = client.GetStream();

        await stream.WriteAsync(commandTypeBytes[commandType], ct);
        if (data.Length > 0)
        {
            await stream.WriteAsync(data, ct);
        }
        
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, ct);
        return ms.ToArray();
    }

    private Task<byte[]> Send(CommandType commandType, CancellationToken ct)
        => this.Send(commandType, Array.Empty<byte>(), ct);
}
