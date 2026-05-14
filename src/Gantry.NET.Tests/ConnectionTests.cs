using System;
using System.Collections.Generic;
using System.Text;

namespace Gantry.NET.Tests;

public class ConnectionTests
{
    [Test]
    public async Task ShouldConnect()
    {
        var testMessage = Guid.NewGuid().ToString();
        var options = new GantryOptions()
        { 
            ConnectionString = "127.0.0.1:2137" 
        };

        var client = new GantryClient(options);
        await client.Put(testMessage);
        var maxOffset = await client.MaxOffset();
        var result = await client.Get(maxOffset);

        Assert.That(result, Is.EqualTo(testMessage), "Should return sent message");
    }
}
