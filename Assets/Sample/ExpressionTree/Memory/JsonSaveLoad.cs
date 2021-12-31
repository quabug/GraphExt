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

    [NotNull] public static IEnumerable<object> Deserialize([NotNull] string json, params Type[] types)
    {
        using var stringReader = new StringReader(json);
        using var jsonReader = new JsonTextReader(stringReader);
        jsonReader.SupportMultipleContent = true;
        foreach (var type in types)
            yield return jsonReader.Read() ? JsonSerializer.Create(_JSON_SERIALIZER_SETTINGS).Deserialize(jsonReader, type) : null;
    }

    public static string Serialize(params object[] objects)
    {
        var json = "";
        foreach (var obj in objects)
        {
            json += JsonConvert.SerializeObject(obj, _JSON_SERIALIZER_SETTINGS);
            json += Environment.NewLine;
        }
        return json;
    }
}

public static class JsonUtility
{
    public static string Serialize<TNode>(this GraphRuntime<TNode> graph) where TNode : INode<GraphRuntime<TNode>>
    {
        return JsonSaveLoad.Serialize(new GraphRuntimeData<TNode>(graph));
    }

    public static GraphRuntime<TNode> Deserialize<TNode>(string json) where TNode : INode<GraphRuntime<TNode>>
    {
        return ((GraphRuntimeData<TNode>) JsonSaveLoad.Deserialize(json, typeof(GraphRuntimeData<TNode>)).Single()).ToMemory();
    }

    internal struct GraphRuntimeData<TNode> where TNode : INode<GraphRuntime<TNode>>
    {
        public Dictionary<Guid, TNode> Nodes;
        public SerializableEdge[] Edges;

        public GraphRuntimeData(GraphRuntime<TNode> graph)
        {
            Nodes = graph.NodeMap.ToDictionary(pair => pair.Key.Id, pair => pair.Value);
            Edges = graph.Edges.Distinct().Select(edge => edge.ToSerializable(graph)).ToArray();
        }

        public GraphRuntime<TNode> ToMemory()
        {
            var graph = new GraphRuntime<TNode>();
            foreach (var pair in Nodes) graph.AddNode(pair.Key, pair.Value);
            foreach (var serializableEdge in Edges)
            {
                try
                {
                    var (input, output) = serializableEdge.ToEdge(graph);
                    graph.Connect(input: input, output: output);
                }
                catch
                {
                    Debug.LogWarning($"invalid edge {serializableEdge.OutputNode}.{serializableEdge.OutputPort}->{serializableEdge.InputNode}.{serializableEdge.InputPort}");
                }
            }
            return graph;
        }
    }
}

#if UNITY_EDITOR
public static class JsonEditorUtility
{
    public static string Serialize<TNode>(this GraphExt.Editor.MemoryGraphViewModule<TNode> graph) where TNode : INode<GraphRuntime<TNode>>
    {
        return JsonSaveLoad.Serialize(new JsonUtility.GraphRuntimeData<TNode>(graph.Runtime), new GraphViewData<TNode>(graph));
    }

    public static GraphExt.Editor.MemoryGraphViewModule<TNode> Deserialize<TNode>(string json)
        where TNode : INode<GraphRuntime<TNode>>
    {
        var dataList = JsonSaveLoad.Deserialize(json, typeof(JsonUtility.GraphRuntimeData<TNode>), typeof(GraphViewData<TNode>)).ToArray();
        var runtimeGraph = ((JsonUtility.GraphRuntimeData<TNode>)dataList[0]).ToMemory();
        return ((GraphViewData<TNode>)dataList[1]).CreateViewModule(runtimeGraph);
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
