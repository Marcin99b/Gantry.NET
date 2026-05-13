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
        var uri = new Uri(options.ConnectionString);
        var data = Encoding.UTF8.GetBytes(command);
        using var client = new TcpClient(uri.Host, uri.Port);
        using var stream = client.GetStream();

        await stream.WriteAsync(data, 0, data.Length);

        var bytes = stream.Read(data, 0, data.Length);
        var response = Encoding.UTF8.GetString(data, 0, bytes);
        return response;
    }
}