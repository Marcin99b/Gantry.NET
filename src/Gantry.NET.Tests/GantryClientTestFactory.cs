namespace Gantry.NET.Tests;

public static class GantryClientTestFactory
{
    public static IGantryClient Create()
    {
        var options = new GantryOptions()
        {
            ConnectionString = "127.0.0.1:2137"
        };

        var client = new GantryClient(options);
        return client;
    }
}