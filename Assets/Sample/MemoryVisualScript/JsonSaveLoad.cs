using System;
using System.IO;
using System.Linq;
using GraphExt;
using GraphExt.Memory;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonSaveLoad
{
    private static readonly JsonSerializerSettings _jsonSerializerSettings;

    static JsonSaveLoad()
    {
        _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };
    }

    public static bool Save(Graph graph, string path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        try
        {
            var json = JsonConvert.SerializeObject(new SerializableGraph(graph), Formatting.Indented, _jsonSerializerSettings);
            File.WriteAllText(path, json);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to save graph to {path}: {ex}");
            return false;
        }
    }

    public static Graph Load(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        try
        {
            var json = File.ReadAllText(path);
            var graph = JsonConvert.DeserializeObject<SerializableGraph>(json, _jsonSerializerSettings);
            return graph.ToMemory();
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to load graph from {path}: {ex}");
            return null;
        }
    }

    struct SerializableNode
    {
        public float PositionX;
        public float PositionY;
        public IMemoryNode Node;

        public SerializableNode(Node node)
        {
            PositionX = node.Position.x;
            PositionY = node.Position.y;
            Node = node.Inner;
        }

        public Node ToMemory()
        {
            return new Node(Node) { Position = new Vector2(PositionX, PositionY) };
        }
    }

    struct SerializableEdge
    {
        public Guid Node1;
        public string Port1;
        public Guid Node2;
        public string Port2;

        public SerializableEdge(in EdgeId edge)
        {
            Node1 = edge.First.NodeId.Id;
            Port1 = edge.First.Name;
            Node2 = edge.Second.NodeId.Id;
            Port2 = edge.Second.Name;
        }

        public EdgeId ToMemory()
        {
            return new EdgeId(new PortId(Node1, Port1), new PortId(Node2, Port2));
        }
    }

    [Serializable]
    struct SerializableGraph
    {
        public SerializableNode[] Nodes;
        public SerializableEdge[] Edges;

        public SerializableGraph(Graph graph)
        {
            Nodes = graph.NodeMap.Select(pair => pair.Value).Select(node => new SerializableNode(node)).ToArray();
            Edges = graph.Edges.Distinct().Select(edge => new SerializableEdge(edge)).ToArray();
        }

        public Graph ToMemory()
        {
            return new Graph(Nodes.Select(node => node.ToMemory()), Edges.Select(edge => edge.ToMemory()));
        }
    }
}