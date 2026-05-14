namespace Gantry.NET;

public class GantryOptions
{
    private string? host;
    private int? port;

    public required string ConnectionString { get; init; }

    public (string Host, int Port) GetAddress()
    {
        if (this.host != null && this.port.HasValue)
        {
            return (this.host, this.port.Value);
        }

        var split = this.ConnectionString.Split(":");
        if (split.Length != 2)
        {
            throw new ArgumentException("Gantry ConnectionString is not in format host:port.");
        }

        var host = split[0];
        if (!int.TryParse(split[1], out var port))
        {
            throw new ArgumentException("Gantry ConnectionString port number is not an integer.");
        }

        this.host = host;
        this.port = port;

        return (this.host, this.port.Value);
    }
}
