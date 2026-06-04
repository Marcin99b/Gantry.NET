using System.Buffers;
using System.Net.Sockets;
using System.Text;

namespace Gantry.NET;

internal class GantryClient(GantryOptions options) : IGantryClient
{
    public async Task<bool> Ping(CancellationToken ct)
        => (await this.Send(CommandType.Ping, ct)).SingleOrDefault() == 1;

    public Task Put(uint topicId, string message, CancellationToken ct) 
        => this.Send(CommandType.PutMessage, topicId, Encoding.UTF8.GetBytes(message), ct);

    public async Task<string> GetAsString(uint topicId, uint offset, CancellationToken ct)
        => Encoding.UTF8.GetString(await this.Get(topicId, offset, ct));

    public Task<byte[]> Get(uint topicId, uint offset, CancellationToken ct)
        => this.Send(CommandType.GetMessage, topicId, BitConverter.GetBytes(offset), ct);

    public async Task<uint> CreateTopic(string name, CancellationToken ct) 
        => BitConverter.ToUInt32(await this.Send(CommandType.CreateTopic, Encoding.UTF8.GetBytes(name), ct));
    public async Task DeleteTopic(string name, CancellationToken ct) 
        => await this.Send(CommandType.DeleteTopic, Encoding.UTF8.GetBytes(name), ct);

    private Task<byte[]> Send(CommandType commandType, byte[] data, CancellationToken ct)
        => this.Send(commandType, 0, data, ct);

    private Task<byte[]> Send(CommandType commandType, CancellationToken ct)
        => this.Send(commandType, 0, Array.Empty<byte>(), ct);

    private async Task<byte[]> Send(CommandType commandType, uint topicId, byte[] data, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var address = options.GetAddress();
        using var client = new TcpClient(address.Host, address.Port);
        using var stream = client.GetStream();

        stream.WriteByte((byte)commandType);
        var topic = topicId == 0 ? Array.Empty<byte>() : BitConverter.GetBytes(topicId);

        await stream.WriteAsync(BitConverter.GetBytes(data.Length + topic.Length), ct);
        
        if (topic.Length > 0)
        {
            await stream.WriteAsync(topic, ct);
        }

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
