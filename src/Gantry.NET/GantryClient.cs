using System.Buffers;
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

    private Task<byte[]> Send(CommandType commandType, CancellationToken ct)
        => this.Send(commandType, Array.Empty<byte>(), ct);

    private async Task<byte[]> Send(CommandType commandType, byte[] data, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var address = options.GetAddress();
        using var client = new TcpClient(address.Host, address.Port);
        client.ReceiveTimeout = 10_000;
        client.SendTimeout = 10_000;
        using var stream = client.GetStream();

        stream.WriteByte((byte)commandType);
        await stream.WriteAsync(BitConverter.GetBytes(data.Length), ct);
        if (data.Length > 0)
        {
            await stream.WriteAsync(data, ct);
        }

        var lenBuf = new byte[4];
        await ReadExactAsync(stream, lenBuf, ct);
        var responseLen = BitConverter.ToInt32(lenBuf);
        var result = new byte[responseLen];
        if (responseLen > 0)
        {
            await ReadExactAsync(stream, result, ct);
        }

        return result;
    }

    private static async Task ReadExactAsync(NetworkStream stream, byte[] buf, CancellationToken ct)
    {
        int offset = 0;
        while (offset < buf.Length)
        {
            int n = await stream.ReadAsync(buf.AsMemory(offset), ct);
            if (n == 0)
            {
                throw new EndOfStreamException();
            }

            offset += n;
        }
    }
}
