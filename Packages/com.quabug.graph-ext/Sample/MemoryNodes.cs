using GraphExt;
using GraphExt.Memory;

public class MemoryFoo : IMemoryNode
{
    [NodeProperty(typeof(FieldInfoProperty<int>))] public int Int;
    [NodeProperty(typeof(ReadOnlyFieldInfoProperty<float>))] public float Float;
    [NodeProperty(typeof(PropertyInfoProperty<double>))] public double Double { get; set; }
    [NodeProperty(typeof(ReadOnlyPropertyInfoProperty<long>))] public long Long { get; }
}