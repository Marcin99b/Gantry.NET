namespace Gantry.NET.Tests;

public class ConnectionTests
{
    [Test]
    public async Task ShouldConnect()
    {
        var client = GantryClientTestFactory.Create();
        var result = await client.Ping(CancellationToken.None);

        Assert.That(result, Is.True);
    }
}
