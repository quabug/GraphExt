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
    [NodePort(typeof(int), Direction = NodePortDirection.Input)] public MemoryPort InputPort;
    [NodePort(typeof(int), Direction = NodePortDirection.Output)] public MemoryPort OutputPort;
}

public class MemoryPort : IMemoryPort
{
    private readonly List<IMemoryPort> _connectedPorts = new List<IMemoryPort>();
    public IReadOnlyList<IMemoryPort> ConnectedPorts => _connectedPorts;

    public bool IsCompatible(IMemoryPort port)
    {
        return true;
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