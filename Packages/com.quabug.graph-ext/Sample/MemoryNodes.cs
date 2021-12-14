using System.Collections.Generic;
using GraphExt.Memory;

public class MemoryFoo : IMemoryNode
{
    [NodeProperty] public int Int = 123;
    [NodeProperty(ReadOnly = true)] public float Float = 1.23f;
    [NodeProperty] public double Double { get; set; } = 3.21;
    [NodeProperty] public long Long { get; } = 321;
    public int HiddenInt = 456;

    [NodePort(typeof(int))] public MemoryPort Port;
    [NodePort(typeof(float), Direction = NodePortDirection.Input)] public MemoryPort InputPort;
    [NodePort(typeof(float), Direction = NodePortDirection.Output)] public MemoryPort OutputPort;

    public MemoryFoo()
    {
        Port = new MemoryPort(this);
        InputPort = new MemoryPort(this);
        OutputPort = new MemoryPort(this);
    }
}

public class MemoryPort : IMemoryPort
{
    public IMemoryNode Node { get; }

    private readonly HashSet<IMemoryPort> _connectedPorts = new HashSet<IMemoryPort>();
    public ISet<IMemoryPort> ConnectedPorts => _connectedPorts;

    public MemoryPort(IMemoryNode node)
    {
        Node = node;
    }

    public bool IsCompatible(IMemoryPort port)
    {
        return !ConnectedPorts.Contains(port) && port.Node != Node;
    }

    public void OnConnected(IMemoryPort port)
    {
        if (!_connectedPorts.Contains(port))
        {
            _connectedPorts.Add(port);
        }
    }

    public void OnDisconnected(IMemoryPort port)
    {
        _connectedPorts.Remove(port);
    }
}