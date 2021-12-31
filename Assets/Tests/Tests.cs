using System;
using System.Linq;
using GraphExt;
using GraphExt.Editor;
using NUnit.Framework;
using UnityEditor.Experimental.GraphView;

public class TestEdge
{
    [Test, Repeat(1000)]
    public void should_be_the_same_between_edges_with_swapped_ports()
    {
        var port1 = new PortId(Guid.NewGuid(), "port");
        var port2 = new PortId(Guid.NewGuid(), "port");
        Assert.AreEqual(new EdgeId(port1, port2), new EdgeId(port2, port1));
        Assert.AreEqual(port1 == port2, new EdgeId(port1, port1) == new EdgeId(port2, port2));
        Assert.AreEqual(port1 != port2, new EdgeId(port1, port1) != new EdgeId(port2, port2));
    }
}

public class TestNodePortAttribute
{
    private class Node : INode<GraphRuntime<Node>>
    {
        [NodePort(Orientation = PortOrientation.Vertical)] public static int InputInt;
        [NodePort] public static float[] OutputFloatMulti;
        [NodePort(DisplayName = "double3")] public static double[] OutputDoulbe3 = new double[3];
        [NodePort(Id = "input-long")] public static long InputLongWithId;
        [NodePort(Direction = PortDirection.Output, Id = "output-short")] public static short ShortWithId;
        [NodePort(Direction = PortDirection.Output, Capacity = 100, DisplayName = "123", PortType = typeof(string))] public static int Port;

        public bool IsPortCompatible(GraphRuntime<Node> graph, in PortId input, in PortId output) { throw new NotImplementedException(); }
        public void OnConnected(GraphRuntime<Node> graph, in PortId input, in PortId output) { throw new NotImplementedException(); }
        public void OnDisconnected(GraphRuntime<Node> graph, in PortId input, in PortId output) { throw new NotImplementedException(); }
    }

    private readonly Node _node = new Node();

    [Test]
    public void should_find_ports()
    {
        var expectedPorts = new PortData[]
        {
            new PortData(nameof(Node.InputInt), Orientation.Vertical, Direction.Input, 1, typeof(int)),
            new PortData(nameof(Node.OutputFloatMulti), Orientation.Horizontal, Direction.Output, int.MaxValue, typeof(float)),
            new PortData(nameof(Node.OutputDoulbe3), Orientation.Horizontal, Direction.Output, 3, typeof(double)),
            new PortData(nameof(Node.InputLongWithId), Orientation.Horizontal, Direction.Input, 1, typeof(long)),
            new PortData(nameof(Node.ShortWithId), Orientation.Horizontal, Direction.Output, 1, typeof(short)),
            new PortData(nameof(Node.Port), Orientation.Horizontal, Direction.Output, 100, typeof(string)),
        };

        var ports = _node.FindPorts().ToArray();

        Assert.That(expectedPorts, Is.EqualTo(ports));
    }

    [Test]
    public void should_find_port_id()
    {
        Assert.AreEqual("input-long", _node.FindSerializedId(nameof(Node.InputLongWithId)));
        Assert.AreEqual("output-short", _node.FindSerializedId(nameof(Node.ShortWithId)));
    }

    [Test]
    public void should_correct_port_name()
    {
        var id = "input-long";
        var name = "unkonwn";
        _node.CorrectIdName(ref id, ref name);
        Assert.AreEqual("input-long", id);
        Assert.AreEqual(nameof(Node.InputLongWithId), name);

        id = "";
        _node.CorrectIdName(ref id, ref name);
        Assert.AreEqual("input-long", id);
        Assert.AreEqual(nameof(Node.InputLongWithId), name);

        id = "123";
        name = "456";
        _node.CorrectIdName(ref id, ref name);
        Assert.AreEqual(null, id);
        Assert.AreEqual(null, name);
    }
}