using GraphExt;
using GraphExt.Memory;

public class MemoryFoo : IMemoryNode
{
    [NodeProperty(typeof(FieldInfoProperty<int>))] public int Int = 123;
    [NodeProperty(typeof(ReadOnlyFieldInfoProperty<float>))] public float Float = 1.23f;
    [NodeProperty(typeof(PropertyInfoProperty<double>))] public double Double { get; set; } = 3.21;
    [NodeProperty(typeof(ReadOnlyPropertyInfoProperty<long>))] public long Long { get; } = 321;
}