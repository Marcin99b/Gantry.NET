using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gantry.NET;

internal class GantryClient(GantryOptions options) : IGantryClient
{
    public async IAsyncEnumerable<string> Iterate(int fromOffset, [EnumeratorCancellation] CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            yield return await this.Get(fromOffset++, ct);
        }
    }

    public Task Put(string message, CancellationToken ct)
    {
        var command = $"put {message}";
        return this.Send(command, ct);
    }

    public Task<string> Get(int offset, CancellationToken ct)
    {
        var command = $"get {offset}";
        return this.Send(command, ct);
    }

    public async Task<int> MaxOffset(CancellationToken ct)
    {
        var command = $"maxoffset";
        var response = await this.Send(command, ct);
        return int.Parse(response);
    }

    private async Task<string> Send(string command, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var address = options.GetAddress();
        var data = Encoding.UTF8.GetBytes(command);
        using var client = new TcpClient(address.Host, address.Port);
        using var stream = client.GetStream();

        await stream.WriteAsync(data, 0, data.Length);

        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, ct);
        var response = Encoding.UTF8.GetString(ms.ToArray());
        
        if (GantryError.TryParse(response, out var error))
        {
            //todo flow with GantryError
            throw new Exception(response);
        }

        return response;
    }
}

public record GantryError(int Code, string Message)
{
    public static bool TryParse(string response, out GantryError? gantryError)
    {
        if (!response.StartsWith("ERROR: "))
        {
            gantryError = null;
            return false;
        }

        response = response.Replace("ERROR: ", "");
        var splitted = response.Split(";");
        var code = int.Parse(splitted[0].Trim());
        var message = splitted[1].Trim();

        gantryError = new GantryError(code, message);
        return true;
    }
}