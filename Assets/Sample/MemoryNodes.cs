using System;
using GraphExt.Memory;

[Serializable]
[NodeTitle]
public class MemoryFoo : IMemoryNode
{
    public Guid Id { get; } = Guid.NewGuid();

    [NodeProperty] public int Int = 123;
    [NodeProperty(ReadOnly = true)] public float Float = 1.23f;
    [NodeProperty] public double Double { get; set; } = 3.21;
    [NodeProperty] public long Long { get; } = 321;

    [NodePort(typeof(int))] public MemoryPort Port = new MemoryPort();
    [NodePort(typeof(float), Direction = NodePortDirection.Input)] public MemoryPort InputPort = new MemoryPort();
    [NodePort(typeof(float), Direction = NodePortDirection.Output)] public MemoryPort OutputPort = new MemoryPort();
}