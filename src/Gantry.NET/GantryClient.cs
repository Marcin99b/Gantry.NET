using System.Net.Sockets;
using System.Text;

namespace Gantry.NET;

internal class GantryClient(GantryOptions options) : IGantryClient
{
    public async Task Put(string message)
    {
        var command = $"put {message}";
        var response =  await this.Send(command);
        if (response != "Ok")
        {
            throw new Exception(response);
        }
    }

    public async Task<string> Get(int offset)
    {
        var command = $"get {offset}";
        var response = await this.Send(command);
        return response;
    }

    public async Task<int> MaxOffset()
    {
        var command = $"maxoffset";
        var response = await this.Send(command);
        return int.Parse(response);
    }

    private async Task<string> Send(string command)
    {
        var address = options.GetAddress();
        var data = Encoding.UTF8.GetBytes(command);
        using var client = new TcpClient(address.Host, address.Port);
        using var stream = client.GetStream();

        await stream.WriteAsync(data, 0, data.Length);

        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return Encoding.UTF8.GetString(ms.ToArray());
    }
}