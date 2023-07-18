using System.Text;

namespace Rivers.Test;

public class RiverPubSubTest
{
    [Test]
    public async Task TestSimpleListen()
    {
        var input = GenerateAsyncEnumerable();
        var river = input.ToRiver();

        var builder = new StringBuilder();

        using var subscription = river.Listen(c => builder.Append(c));

        await river.ToListAsync();

        subscription.Cancel();

        Assert.That(builder.ToString(), Is.EqualTo("test"));
    }

    [Test]
    public async Task TestRiverIsAsyncEnumerable()
    {
        var input = GenerateAsyncEnumerable().ToRiver();

        await foreach (var value in input)
        {
            Assert.That(value, Is.EqualTo("test"));
        }
    }

    [Test]
    public async Task TestAsTask()
    {
        var input = GenerateAsyncEnumerable();
        var river = input.ToRiver();
        using var subscription = river.Listen();
        var task = subscription.AsTask();

        Assert.That(task.Status, Is.EqualTo(TaskStatus.WaitingForActivation));

        await river.ToListAsync();
        await task;
    }

    [Test]
    public async Task TestAsTaskFallback()
    {
        var input = GenerateAsyncEnumerable("");
        var river = input.ToRiver();
        using var subscription = river.Listen();
        var task = subscription.AsTask('0');

        Assert.That(task.Status, Is.EqualTo(TaskStatus.WaitingForActivation));

        await river.ToListAsync();
        
        var result = await task;
        
        Assert.That(result, Is.EqualTo('0'));
    }

    [Test]
    public void TestToRiverReturnsSameInstance()
    {
        var input = GenerateAsyncEnumerable();
        var a = input.ToRiver();
        var b = a.ToRiver();

        Assert.That(b, Is.SameAs(a));
    }

    [Test]
    public void TestExceptionGetsHandled()
    {
        var input = GenerateAsyncEnumerable(shouldThrow: true).ToRiver();
        Exception? ex = null;
        input.Listen(onError: e => ex = e);

        Assert.ThrowsAsync<Exception>(async () => await input.ToListAsync());

        Assert.That(ex?.Message, Is.EqualTo("exception"));
    }

    [Test]
    public void TestDefaultExceptionHandler()
    {
        var input = GenerateAsyncEnumerable(shouldThrow: true).ToRiver();
        input.Listen();

        Assert.ThrowsAsync<Exception>(async () => await input.ToListAsync());
    }

    [Test]
    public async Task TestAsTaskCancellation()
    {
        var input = GenerateAsyncEnumerable().ToRiver();

        var output = "";
        var subscription = input.Listen(v => output += v);

        var cts = new CancellationTokenSource();
        _ = subscription.AsTask(cts.Token);
        cts.Cancel();

        await input.ToListAsync();

        Assert.That(output, Is.Empty);
    }

    [Test]
    public async Task TestAsTaskWithValueCancellation()
    {
        var input = GenerateAsyncEnumerable().ToRiver();

        var output = "";
        var subscription = input.Listen(v => output += v);

        var cts = new CancellationTokenSource();
        _ = subscription.AsTask(true, cts.Token);
        cts.Cancel();

        await input.ToListAsync();

        Assert.That(output, Is.Empty);
    }

    [Test]
    public async Task TastTaskThrows()
    {
        var input = GenerateAsyncEnumerable(shouldThrow: true).ToRiver();

        var subscription = input.Listen();
        var noValueTask = subscription.AsTask();

        Assert.ThrowsAsync<Exception>(async () => await input.ToListAsync());
        Assert.ThrowsAsync<Exception>(async () => await noValueTask);
    }

    [Test]
    public async Task TastTaskWithValueThrows()
    {
        var input = GenerateAsyncEnumerable(shouldThrow: true).ToRiver();

        var subscription = input.Listen();
        var valueTask = subscription.AsTask(true);

        Assert.ThrowsAsync<Exception>(async () => await input.ToListAsync());
        Assert.ThrowsAsync<Exception>(async () => await valueTask);
    }

    [Test]
    public void TestEnumeratorGetsReused()
    {
        var enumerable = GenerateAsyncEnumerable();
        var enumerator = enumerable.GetAsyncEnumerator();
        var notifyingEnumerator = new NotifyingAsyncEnumerator<string>(enumerator);

        var river = new River<string>(notifyingEnumerator);
        var riverEnumerator = river.GetAsyncEnumerator();

        Assert.That(riverEnumerator, Is.SameAs(notifyingEnumerator));
    }

    private static async IAsyncEnumerable<string> GenerateAsyncEnumerable(string source = "test", int count = 1, bool shouldThrow = false)
    {
        if (shouldThrow)
            throw new("exception");

        for (var i = 0; i < count; i++)
            yield return source;
    }
}