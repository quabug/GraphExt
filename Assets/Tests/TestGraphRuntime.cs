using System;
using System.Collections.Generic;
using System.Linq;
using GraphExt;
using NUnit.Framework;

public class TestGraphRuntime
{
    [Test]
    public void should_add_and_then_delete_nodes([Random(10, 100, 100)] int nodeCount)
    {
        var nodes = NodeIdsAndNodes().Take(nodeCount).ToList();
        var graph = new GraphRuntime<TestNode>();
        var callbackNodes = new List<(NodeId id, TestNode)>();
        graph.OnNodeAdded += (in NodeId id, TestNode node) => callbackNodes.Add((id, node));
        graph.OnNodeWillDelete += (in NodeId id, TestNode node) => callbackNodes.Remove((id, node));

        foreach (var (id, node) in nodes) graph.AddNode(id, node);
        Assert.IsEmpty(graph.Edges);
        Assert.That(nodes, Is.EqualTo(callbackNodes));
        CheckGraphNodes(graph, nodes);

        var maxRemoveCount = _random.Next(nodes.Count * 2);
        while (maxRemoveCount > 0 && nodes.Any())
        {
            var nodeIndex = _random.Next(nodes.Count);
            var (id, _) = nodes[nodeIndex];
            graph.DeleteNode(id);
            nodes.RemoveAt(nodeIndex);
            maxRemoveCount--;
        }
        Assert.IsEmpty(graph.Edges);
        Assert.That(nodes, Is.EqualTo(callbackNodes));
        CheckGraphNodes(graph, nodes);
    }

    [Test]
    public void should_throw_if_connect_invalid_port()
    {
        var graph = new GraphRuntime<TestNode>();
        var (input, output) = EdgeIds().First();
        Assert.Catch<InvalidPortException>(() => graph.Connect(input, output));
    }

    [Test]
    public void should_throw_if_disconnect_invalid_port()
    {
        var graph = new GraphRuntime<TestNode>();
        var (input, output) = EdgeIds().First();
        Assert.Catch<InvalidPortException>(() => graph.Disconnect(input, output));
    }

    [Test]
    public void should_throw_if_edge_already_connected()
    {
        var graph = new GraphRuntime<TestNode>();
        var (nodeId, node) = NodeIdsAndNodes().First();
        var input = PortIds(nodeId).First();
        var output = PortIds(nodeId).First();
        graph.AddNode(nodeId, node);
        graph.Connect(input, output);
        Assert.Catch<EdgeAlreadyConnectedException>(() => graph.Connect(input, output));
    }

    [Test]
    public void should_throw_if_edge_already_disconnected()
    {
        var graph = new GraphRuntime<TestNode>();
        var (nodeId, node) = NodeIdsAndNodes().First();
        var input = PortIds(nodeId).First();
        var output = PortIds(nodeId).First();
        graph.AddNode(nodeId, node);
        Assert.Catch<EdgeAlreadyDisconnectedException>(() => graph.Disconnect(input, output));
    }

    [Test]
    public void should_connect_and_then_disconnect([Random(1, 10, 100)] int nodeCount)
    {
        var nodes = NodeIdsAndNodes().Take(nodeCount).ToList();
        var ports = new GraphExt.HashSet<PortId>(nodes.SelectMany(node => PortIds(node.id).Take(_random.Next(10))));
        var graph = ConstructGraph(nodes);
        var edges = Shuffle(EdgeIds(ports).ToList());
        var edgeSet = new GraphExt.HashSet<EdgeId>(edges.Take(_random.Next(edges.Count * 2)));

        var connected = new Queue<(NodeId id, PortId input, PortId output)>();
        var disconnected = new Queue<(NodeId id, PortId input, PortId output)>();

        foreach (var node in nodes)
        {
            node.node.Connected += (g, input, output) =>
            {
                Assert.AreEqual(g, graph);
                connected.Enqueue((node.id, input, output));
            };
            node.node.Disconnected += (g, input, output) =>
            {
                Assert.AreEqual(g, graph);
                disconnected.Enqueue((node.id, input, output));
            };
        }

        var callbackEdgeSet = new GraphExt.HashSet<EdgeId>();
        graph.OnEdgeConnected += (in EdgeId edge) => callbackEdgeSet.Add(edge);
        graph.OnEdgeWillDisconnect += (in EdgeId edge) => callbackEdgeSet.Remove(edge);

        foreach (var edge in edgeSet)
        {
            graph.Connect(edge.Input, edge.Output);
            Assert.AreEqual(2, connected.Count);
            Assert.AreEqual((edge.Input.NodeId, edge.Input, edge.Output), connected.Dequeue());
            Assert.AreEqual((edge.Output.NodeId, edge.Input, edge.Output), connected.Dequeue());
        }
        CheckGraphNodes(graph, nodes);
        Assert.That(graph.Edges, Is.EqualTo(edgeSet));
        Assert.That(callbackEdgeSet, Is.EqualTo(edgeSet));

        var removedSet = Shuffle(edgeSet.ToList()).ToHashSet();
        foreach (var edge in removedSet)
        {
            graph.Disconnect(edge.Input, edge.Output);
            edgeSet.Remove(edge);
            Assert.AreEqual(2, disconnected.Count);
            Assert.AreEqual((edge.Input.NodeId, edge.Input, edge.Output), disconnected.Dequeue());
            Assert.AreEqual((edge.Output.NodeId, edge.Input, edge.Output), disconnected.Dequeue());
        }
        CheckGraphNodes(graph, nodes);
        Assert.That(graph.Edges, Is.EqualTo(edgeSet));
        Assert.That(callbackEdgeSet, Is.EqualTo(edgeSet));
    }

    [Test]
    public void should_disconnect_edges_once_node_was_deleted([Random(1, 10, 100)] int nodeCount)
    {
        var nodes = NodeIdsAndNodes().Take(nodeCount).ToList();
        var ports = new GraphExt.HashSet<PortId>(nodes.SelectMany(node => PortIds(node.id).Take(_random.Next(10))));
        var edges = EdgeIds(ports).ToHashSet();
        var graph = ConstructGraph(nodes, edges);
        Assert.That(graph.Edges, Is.EqualTo(edges));

        var nodeEdges = nodes.ToDictionary(node => node.id, _ => new GraphExt.HashSet<EdgeId>());
        foreach (var edge in edges)
        {
            nodeEdges[edge.Input.NodeId].Add(edge);
            nodeEdges[edge.Output.NodeId].Add(edge);
        }

        foreach (var node in nodes)
        {
            graph.DeleteNode(node.id);
            var set = nodeEdges[node.id];
            Assert.IsFalse(set.Overlaps(graph.Edges));
        }
    }
    //
    // [Test]
    // public void should_check_port_compatible()
    // {
    //     var nodes = NodeIdsAndNodes().Take(2).ToList();
    //     var port1 = PortIds(nodes[0].id).First();
    //     var port2 = PortIds(nodes[1].id).First();
    //     var compatible = (graph: true, node1: true, node2: true);
    //     var graph = new GraphRuntime<TestNode>((in PortId input, in PortId output) =>
    //     {
    //         Assert.AreEqual(input, port1);
    //         Assert.AreEqual(output, port2);
    //         return compatible.graph;
    //     });
    //     nodes[0].node.IsPortMatch += (g, input, output) =>
    //     {
    //         Assert.AreEqual(g, graph);
    //         Assert.AreEqual(input, port1);
    //         Assert.AreEqual(output, port2);
    //         return compatible.node1;
    //     };
    //     nodes[1].node.IsPortMatch += (g, input, output) =>
    //     {
    //         Assert.AreEqual(g, graph);
    //         Assert.AreEqual(input, port1);
    //         Assert.AreEqual(output, port2);
    //         return compatible.node2;
    //     };
    //     foreach (var (id, node) in nodes) graph.AddNode(id, node);
    //
    //     Assert.IsTrue(graph.IsCompatible(port1, port2));
    //
    //     compatible.graph = false;
    //     compatible.node1 = true;
    //     compatible.node2 = true;
    //     Assert.IsFalse(graph.IsCompatible(port1, port2));
    //
    //     compatible.graph = true;
    //     compatible.node1 = false;
    //     compatible.node2 = true;
    //     Assert.IsFalse(graph.IsCompatible(port1, port2));
    //
    //     compatible.graph = true;
    //     compatible.node1 = true;
    //     compatible.node2 = false;
    //     Assert.IsFalse(graph.IsCompatible(port1, port2));
    // }

    private readonly Random _random = new Random();

    GraphRuntime<TestNode> ConstructGraph(IEnumerable<(NodeId id, TestNode node)> nodes, IEnumerable<EdgeId> edges = null)
    {
        var graph = new GraphRuntime<TestNode>();
        foreach (var (id, node) in nodes) graph.AddNode(id, node);
        if (edges != null) foreach (var edge in edges) graph.Connect(edge.Input, edge.Output);
        return graph;
    }

    class TestNode : INode<GraphRuntime<TestNode>>
    {
        public event Func<GraphRuntime<TestNode>, PortId, PortId, bool> IsPortMatch = (graph, input, output) => true;
        public event Action<GraphRuntime<TestNode>, PortId, PortId> Connected =  (graph, input, output) => {};
        public event Action<GraphRuntime<TestNode>, PortId, PortId> Disconnected =  (graph, input, output) => {};
        public bool IsPortCompatible(GraphRuntime<TestNode> graph, in PortId input, in PortId output) => IsPortMatch(graph, input, output);
        public void OnConnected(GraphRuntime<TestNode> graph, in PortId input, in PortId output) => Connected(graph, input, output);
        public void OnDisconnected(GraphRuntime<TestNode> graph, in PortId input, in PortId output) => Disconnected(graph, input, output);
    }

    KeyValuePair<NodeId, TestNode> ToPair((NodeId id, TestNode node) item) =>
        new KeyValuePair<NodeId, TestNode>(item.id, item.node);

    KeyValuePair<TestNode, NodeId> ToReversePair((NodeId id, TestNode node) item) =>
        new KeyValuePair<TestNode, NodeId>(item.node, item.id);

    IEnumerable<NodeId> NodeIds()
    {
        while (true) yield return Guid.NewGuid();
    }

    IEnumerable<TestNode> Nodes()
    {
        while (true) yield return new TestNode();
    }

    IEnumerable<(NodeId id, TestNode node)> NodeIdsAndNodes()
    {
        return NodeIds().Zip(Nodes(), (id, node) => (id, node));
    }

    IEnumerable<PortId> PortIds()
    {
        return NodeIds().Select(nodeId => new PortId(nodeId, Guid.NewGuid().ToString()));
    }

    IEnumerable<PortId> PortIds(NodeId nodeId)
    {
        while (true) yield return new PortId(nodeId, Guid.NewGuid().ToString());
    }

    IEnumerable<EdgeId> EdgeIds()
    {
        return PortIds().Zip(PortIds(), (input, output) => new EdgeId(input, output));
    }

    IEnumerable<EdgeId> EdgeIds(IReadOnlyCollection<PortId> ports) =>
        from input in ports
        from output in ports
        select new EdgeId(input, output);

    void CheckGraphNodes(GraphRuntime<TestNode> graph, IReadOnlyCollection<(NodeId id, TestNode node)> nodes)
    {
        Assert.That(graph.NodeMap, Is.EqualTo(nodes.Select(ToPair)));
        Assert.That(graph.NodeIdMap, Is.EqualTo(nodes.Select(ToReversePair)));
        foreach (var (id, node) in nodes)
        {
            Assert.AreEqual(graph[id], node);
            Assert.AreEqual(graph[node], id);
        }
    }

    List<T> Shuffle<T>(List<T> list)
    {
        var n = list.Count;
        while (n > 1) {
            n--;
            var k = _random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
        return list;
    }
}