using System;
using GraphExt;
using NUnit.Framework;

public class Tests
{
    [Test, Repeat(1000)]
    public void TestsSimplePasses()
    {
        var port1 = Guid.NewGuid();
        var port2 = Guid.NewGuid();
        Assert.AreEqual(new EdgeId(port1, port2), new EdgeId(port2, port1));
        Assert.AreEqual(port1 == port2, new EdgeId(port1, port1) == new EdgeId(port2, port2));
        Assert.AreEqual(port1 != port2, new EdgeId(port1, port1) != new EdgeId(port2, port2));
    }
}
