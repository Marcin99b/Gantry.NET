namespace Gantry.NET.Tests;

public class MessagesTests
{
    [Test]
    public async Task ShouldReturnInsertedMessage()
    {
        var message = Guid.NewGuid().ToString();
        var client = GantryClientTestFactory.Create();

        await client.Put(message, CancellationToken.None);
        var result = await client.GetAsString(0, CancellationToken.None);

        Assert.That(result, Is.EqualTo(message));
    }

    [Test]
    public async Task ShouldReturnAllInsertedMessages_OrderNotRequired()
    {
        var count = 10;
        var messages = Enumerable.Range(0, count).Select(x => Guid.NewGuid().ToString()).ToArray();
        var client = GantryClientTestFactory.Create();

        await Task.WhenAll(messages.Select(x => client.Put(x, CancellationToken.None)));

        var results = await Task.WhenAll(Enumerable.Range(0, count).Select(x => client.GetAsString(x, CancellationToken.None)));

        messages.Sort();
        results.Sort();

        Assert.That(results, Is.EquivalentTo(messages));
    }
}
