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
    public static bool Save<TNode>(this GraphRuntime<TNode> graph, string path) where TNode : INode<GraphRuntime<TNode>>
    {
        try
        {
            JsonSaveLoad.Save(path, new GraphRuntimeData<TNode>(graph));
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to save {nameof(GraphRuntime<TNode>)} to {path}: {ex}");
            return false;
        }
    }

    public static GraphRuntime<TNode> Load<TNode>(string json) where TNode : INode<GraphRuntime<TNode>>
    {
        try
        {
            return ((GraphRuntimeData<TNode>) JsonSaveLoad.LoadJson(json, typeof(GraphRuntimeData<TNode>)).Single()).ToMemory();
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to load {nameof(GraphRuntime<TNode>)} from json string: {ex}");
            return new GraphRuntime<TNode>();
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

    internal struct GraphRuntimeData<TNode> where TNode : INode<GraphRuntime<TNode>>
    {
        public Dictionary<Guid, TNode> Nodes;
        public SerializableEdge[] Edges;

        public GraphRuntimeData(GraphRuntime<TNode> graph)
        {
            Nodes = graph.NodeMap.ToDictionary(pair => pair.Key.Id, pair => pair.Value);
            Edges = graph.Edges.Distinct().Select(edge => new SerializableEdge(edge)).ToArray();
        }

        public GraphRuntime<TNode> ToMemory()
        {
            var graph = new GraphRuntime<TNode>();
            foreach (var pair in Nodes)
                graph.AddNode(pair.Key, pair.Value);
            foreach (var edge in Edges.Select(e => e.ToMemory()))
                graph.Connect(input: edge.Input, output: edge.Output);
            return graph;
        }
    }
}

#if UNITY_EDITOR
public static class JsonEditorUtility
{
    public static bool Save<TNode>(this GraphExt.Editor.MemoryGraphViewModule<TNode> graph, string path)
        where TNode : INode<GraphRuntime<TNode>>
    {
        try
        {
            JsonSaveLoad.Save(path, new JsonUtility.GraphRuntimeData<TNode>(graph.Runtime), new GraphViewData<TNode>(graph));
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to save {nameof(GraphExt.Editor.MemoryGraphViewModule<TNode>)} to {path}: {ex}");
            return false;
        }
    }

    public static GraphExt.Editor.MemoryGraphViewModule<TNode> Load<TNode>(string path)
        where TNode : INode<GraphRuntime<TNode>>
    {
        try
        {
            var dataList = JsonSaveLoad.LoadFile(path, typeof(JsonUtility.GraphRuntimeData<TNode>), typeof(GraphViewData<TNode>)).ToArray();
            var runtimeGraph = ((JsonUtility.GraphRuntimeData<TNode>)dataList[0]).ToMemory();
            return ((GraphViewData<TNode>)dataList[1]).CreateViewModule(runtimeGraph);
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to load {nameof(GraphExt.Editor.MemoryGraphViewModule<TNode>)} from {path}: {ex}");
            return new GraphExt.Editor.MemoryGraphViewModule<TNode>();
        }
    }

    struct GraphViewData<TNode> where TNode : INode<GraphRuntime<TNode>>
    {
        public Dictionary<Guid, (float x, float y)> Positions;

        public GraphViewData(GraphExt.Editor.MemoryGraphViewModule<TNode> graph)
        {
            Positions = graph.NodePositions.ToDictionary(t => t.id.Id, t => (t.position.x, t.position.y));
        }

        public GraphExt.Editor.MemoryGraphViewModule<TNode> CreateViewModule(GraphRuntime<TNode> runtimeData)
        {
            return new GraphExt.Editor.MemoryGraphViewModule<TNode>(
                runtimeData,
                Positions.ToDictionary(pair => new NodeId(pair.Key), pair => new Vector2(pair.Value.x, pair.Value.y))
            );
        }
    }
}
#endif
