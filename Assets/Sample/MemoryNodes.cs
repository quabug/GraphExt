using System;
using GraphExt.Memory;

[Serializable]
[NodeTitle]
public class MemoryFoo : MemoryNode
{
    [NodeProperty(OutputPort = nameof(_intOut))] public int Int = 123;
    [NodePort] private static int _intOut;

    [NodeProperty(ReadOnly = true, InputPort = nameof(_floatIn), OutputPort = nameof(_floatOut))] public float Float = 1.23f;
    [NodePort] private static float _floatIn;
    [NodePort] private static float _floatOut;

    [NodeProperty] public double Double { get; set; } = 3.21;

    [NodePort(Direction = PortDirection.Input, Capacity = PortCapacity.Single, Name = "String")] private static string _StringIn;

    [NodeProperty] public long Long { get; } = 321;
}