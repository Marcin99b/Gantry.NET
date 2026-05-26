using System.Security.Cryptography;

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
    public async Task ShouldReturnInsertedMessage_Long()
    {
        var message = RandomNumberGenerator.GetHexString(10_000);
        var client = GantryClientTestFactory.Create();

        await client.Put(message, CancellationToken.None);
        var result = await client.GetAsString(0, CancellationToken.None);

        Assert.That(result, Is.EqualTo(message));
    }

    [TestCase(3)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1_000)]
    [TestCase(10_000)]
    public async Task ShouldReturnAllInsertedMessages_OrderNotRequired(int count)
    {
        var messages = Enumerable.Range(0, count).Select(x => Guid.NewGuid().ToString()).ToArray();
        var client = GantryClientTestFactory.Create();

        foreach (var message in messages)
        {
            await client.Put(message, CancellationToken.None);
        }

        for (var i = 0; i < count; i++)
        {
            var result = await client.GetAsString(i, CancellationToken.None);
            Assert.That(result, Is.EqualTo(messages[i]));
        }
    }
}
