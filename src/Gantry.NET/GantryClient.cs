using System.Net.Sockets;
using System.Text;

namespace Gantry.NET;

internal class GantryClient(GantryOptions options) : IGantryClient
{
    public async Task<bool> Ping(CancellationToken ct)
        => (await this.Send(CommandType.Ping, ct)).SingleOrDefault() == 1;

    public Task Put(string message, CancellationToken ct) 
        => this.Send(CommandType.PutMessage, Encoding.UTF8.GetBytes(message), ct);

    public async Task<string> GetAsString(int offset, CancellationToken ct)
        => Encoding.UTF8.GetString(await this.Get(offset, ct));

    public Task<byte[]> Get(int offset, CancellationToken ct)
        => this.Send(CommandType.GetMessage, BitConverter.GetBytes(offset), ct);

    private async Task<byte[]> Send(CommandType commandType, byte[] data, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var address = options.GetAddress();
        using var client = new TcpClient(address.Host, address.Port);
        using var stream = client.GetStream();

        var bytes = new byte[data.Length + 1];
        bytes[0] = (byte)commandType;
        for (var i = 0; i < data.Length; i++)
        {
            bytes[i + 1] = data[i];
        }

        await stream.WriteAsync(bytes, ct);
        
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, ct);
        return ms.ToArray();
    }

    private Task<byte[]> Send(CommandType commandType, CancellationToken ct)
        => this.Send(commandType, Array.Empty<byte>(), ct);
}
