using GraphExt.Memory;

public class MemoryFoo : IMemoryNode
{
    [NodeProperty] public int Int = 123;
    [NodeProperty(ReadOnly = true)] public float Float = 1.23f;
    [NodeProperty] public double Double { get; set; } = 3.21;
    [NodeProperty] public long Long { get; } = 321;
}