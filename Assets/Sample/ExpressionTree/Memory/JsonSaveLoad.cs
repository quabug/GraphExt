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

    internal struct SerializableEdge
    {
        public Guid InputNode;
        public string InputPort;
        public string InputPortId;
        public Guid OutputNode;
        public string OutputPort;
        public string OutputPortId;

        public SerializableEdge(in EdgeId edge, string inputPortId, string outputPortId)
        {
            InputNode = edge.Input.NodeId.Id;
            InputPort = edge.Input.Name;
            OutputNode = edge.Output.NodeId.Id;
            OutputPort = edge.Output.Name;
            InputPortId = inputPortId;
            OutputPortId = outputPortId;
        }

        public EdgeId? ToMemory<TNode>(GraphRuntime<TNode> graph) where TNode : INode<GraphRuntime<TNode>>
        {
#if UNITY_EDITOR || ENABLE_RUNTIME_PORT_NAME_CORRECTION
            graph[InputNode].CorrectIdName(portId: ref InputPortId, portName: ref InputPort);
            graph[OutputNode].CorrectIdName(portId: ref OutputPortId, portName: ref OutputPort);
            if (string.IsNullOrEmpty(InputPort) || string.IsNullOrEmpty(OutputPort)) return null;
#endif
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
            Edges = graph.Edges.Distinct().Select(edge =>
            {
                var inputPortId = "";
                var outputPortId = "";
#if UNITY_EDITOR || ENABLE_RUNTIME_PORT_NAME_CORRECTION
                inputPortId = graph[edge.Input.NodeId].FindSerializedId(edge.Input.Name);
                outputPortId = graph[edge.Output.NodeId].FindSerializedId(edge.Output.Name);
#endif
                return new SerializableEdge(edge, inputPortId: inputPortId, outputPortId: outputPortId);
            }).ToArray();
        }

        public GraphRuntime<TNode> ToMemory()
        {
            var graph = new GraphRuntime<TNode>();
            foreach (var pair in Nodes) graph.AddNode(pair.Key, pair.Value);
            foreach (var edge in Edges.Select(e => e.ToMemory(graph)).Where(edge => edge.HasValue))
                graph.Connect(input: edge.Value.Input, output: edge.Value.Output);
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
