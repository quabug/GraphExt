using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GraphExt;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonSaveLoad
{
    private static readonly JsonSerializerSettings _JSON_SERIALIZER_SETTINGS;

    static JsonSaveLoad()
    {
        _JSON_SERIALIZER_SETTINGS = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };
    }

    [NotNull] public static IEnumerable<object> LoadFile([NotNull] string path, params Type[] types)
    {
        using var textReader = File.OpenText(path);
        using var reader = new JsonTextReader(textReader);
        reader.SupportMultipleContent = true;
        foreach (var type in types)
            yield return reader.Read() ? JsonSerializer.Create(_JSON_SERIALIZER_SETTINGS).Deserialize(reader, type) : null;
    }

    [NotNull] public static IEnumerable<object> LoadJson([NotNull] string json, params Type[] types)
    {
        using var textReader = new StringReader(json);
        using var reader = new JsonTextReader(textReader);
        reader.SupportMultipleContent = true;
        foreach (var type in types)
            yield return reader.Read() ? JsonSerializer.Create(_JSON_SERIALIZER_SETTINGS).Deserialize(reader, type) : null;
    }

    public static void Save(string path, params object[] objects)
    {
        using var file = File.CreateText(path);
        foreach (var obj in objects)
        {
            file.Write(JsonConvert.SerializeObject(obj, _JSON_SERIALIZER_SETTINGS));
            file.WriteLine();
        }
    }
}

public static class JsonUtility
{
    public static bool Save(this GraphRuntime<IMemoryNode> graph, string path)
    {
        try
        {
            JsonSaveLoad.Save(path, new GraphRuntimeData(graph));
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to save {nameof(GraphRuntime<IMemoryNode>)} to {path}: {ex}");
            return false;
        }
    }

    public static GraphRuntime<IMemoryNode> Load(string json)
    {
        try
        {
            return ((GraphRuntimeData) JsonSaveLoad.LoadJson(json, typeof(GraphRuntimeData)).Single()).ToMemory();
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to load {nameof(GraphRuntime<IMemoryNode>)} from json string: {ex}");
            return new GraphRuntime<IMemoryNode>();
        }
    }

    internal struct SerializableEdge
    {
        public Guid InputNode;
        public string InputPort;
        public Guid OutputNode;
        public string OutputPort;

        public SerializableEdge(in EdgeId edge)
        {
            InputNode = edge.Input.NodeId.Id;
            InputPort = edge.Input.Name;
            OutputNode = edge.Output.NodeId.Id;
            OutputPort = edge.Output.Name;
        }

        public EdgeId ToMemory()
        {
            return new EdgeId(input: new PortId(InputNode, InputPort), output: new PortId(OutputNode, OutputPort));
        }
    }

    internal struct GraphRuntimeData
    {
        public Dictionary<Guid, IMemoryNode> Nodes;
        public SerializableEdge[] Edges;

        public GraphRuntimeData(GraphRuntime<IMemoryNode> graph)
        {
            Nodes = graph.NodeMap.ToDictionary(pair => pair.Key.Id, pair => pair.Value);
            Edges = graph.Edges.Distinct().Select(edge => new SerializableEdge(edge)).ToArray();
        }

        public GraphRuntime<IMemoryNode> ToMemory()
        {
            var graph = new GraphRuntime<IMemoryNode>();
            foreach (var pair in Nodes)
                graph.AddNode(pair.Key, pair.Value, NodePortAttribute.FindPortNames(pair.Value.GetType()));
            foreach (var edge in Edges.Select(e => e.ToMemory()))
                graph.Connect(input: edge.Input, output: edge.Output);
            return graph;
        }
    }
}

#if UNITY_EDITOR
public static class JsonEditorUtility
{
    public static bool Save(this GraphExt.Editor.MemoryGraphViewModule graph, string path)
    {
        try
        {
            JsonSaveLoad.Save(path, new JsonUtility.GraphRuntimeData(graph.Runtime), new GraphViewData(graph));
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to save {nameof(GraphExt.Editor.MemoryGraphViewModule)} to {path}: {ex}");
            return false;
        }
    }

    public static GraphExt.Editor.MemoryGraphViewModule Load(string path)
    {
        try
        {
            var dataList = JsonSaveLoad.LoadFile(path, typeof(JsonUtility.GraphRuntimeData), typeof(GraphViewData)).ToArray();
            var runtimeGraph = ((JsonUtility.GraphRuntimeData)dataList[0]).ToMemory();
            return ((GraphViewData)dataList[1]).CreateViewModule(runtimeGraph);
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to load {nameof(GraphExt.Editor.MemoryGraphViewModule)} from {path}: {ex}");
            return new GraphExt.Editor.MemoryGraphViewModule();
        }
    }

    struct GraphViewData
    {
        public Dictionary<Guid, (float x, float y)> Positions;

        public GraphViewData(GraphExt.Editor.MemoryGraphViewModule graph)
        {
            Positions = graph.NodePositions.ToDictionary(t => t.id.Id, t => (t.position.x, t.position.y));
        }

        public GraphExt.Editor.MemoryGraphViewModule CreateViewModule(GraphRuntime<IMemoryNode> runtimeData)
        {
            return new GraphExt.Editor.MemoryGraphViewModule(
                runtimeData,
                Positions.Select(pair => (new NodeId(pair.Key), new Vector2(pair.Value.x, pair.Value.y)))
            );
        }
    }
}
#endif
