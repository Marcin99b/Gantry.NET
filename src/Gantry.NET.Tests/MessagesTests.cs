using System.Security.Cryptography;

namespace Gantry.NET.Tests;

public class MessagesTests
{
    [Retry(1)]
    [CancelAfter(5_000)]
    [Test]
    public async Task ShouldReturnInsertedMessage()
    {
        var topicId = (uint) RandomNumberGenerator.GetInt32(1, int.MaxValue);

        var message = Guid.NewGuid().ToString();
        var client = GantryClientTestFactory.Create();

        await client.Put(topicId, message, CancellationToken.None);
        var result = await client.GetAsString(topicId, 0, CancellationToken.None);

        Assert.That(result, Is.EqualTo(message));
    }

    [Retry(1)]
    [CancelAfter(5_000)]
    [Test]
    public async Task ShouldReturnInsertedMessage_Long()
    {
        var topicId = (uint)RandomNumberGenerator.GetInt32(1, int.MaxValue);

        var message = RandomNumberGenerator.GetHexString(10_000);
        var client = GantryClientTestFactory.Create();

        await client.Put(topicId, message, CancellationToken.None);
        var result = await client.GetAsString(topicId, 0, CancellationToken.None);

        Assert.That(result, Is.EqualTo(message));
    }

    [Retry(1)]
    [CancelAfter(5_000)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1_000)]
    [TestCase(10_000)]
    public async Task ShouldReturnAllInsertedMessages_OrderNotRequired(int count)
    {
        var topicId = (uint)RandomNumberGenerator.GetInt32(1, int.MaxValue);

        var messages = Enumerable.Range(0, count).Select(x => Guid.NewGuid().ToString()).ToArray();
        var client = GantryClientTestFactory.Create();

        await Task.WhenAll(messages.Select(x => client.Put(topicId, x, CancellationToken.None)));

        var results = await Task.WhenAll(Enumerable.Range(0, count).Select(x => client.GetAsString(topicId, (uint)x, CancellationToken.None)));

        Assert.That(results.OrderBy(x => x), Is.EquivalentTo(messages.OrderBy(x => x)));
    }
}
