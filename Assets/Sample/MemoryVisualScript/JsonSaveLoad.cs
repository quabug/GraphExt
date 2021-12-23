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

    public static bool Save(MemoryGraphBackend memoryGraphBackend, string path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        try
        {
            var json = JsonConvert.SerializeObject(new SerializableGraph(memoryGraphBackend), Formatting.Indented, _jsonSerializerSettings);
            File.WriteAllText(path, json);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to save memoryGraphBackend to {path}: {ex}");
            return false;
        }
    }

    public static MemoryGraphBackend Load(string path)
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
            Debug.LogError($"failed to load MemoryGraphBackend from {path}: {ex}");
            return null;
        }
    }

    struct SerializableNode
    {
        public float PositionX;
        public float PositionY;
        public Guid Id;
        public IMemoryNode Inner;

        public SerializableNode(MemoryGraphBackend.Node node)
        {
            PositionX = node.Position.x;
            PositionY = node.Position.y;
            Id = node.Id.Id;
            Inner = node.Inner;
        }

        public MemoryGraphBackend.Node ToMemory()
        {
            return new MemoryGraphBackend.Node(Inner, Id, new Vector2(PositionX, PositionY));
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

        public SerializableGraph(MemoryGraphBackend memoryGraphBackend)
        {
            Nodes = memoryGraphBackend.MemoryNodeMap.Select(pair => pair.Value).Select(node => new SerializableNode(node)).ToArray();
            Edges = memoryGraphBackend.Edges.Distinct().Select(edge => new SerializableEdge(edge)).ToArray();
        }

        public MemoryGraphBackend ToMemory()
        {
            return new MemoryGraphBackend(Nodes.Select(node => node.ToMemory()).ToArray(), Edges.Select(edge => edge.ToMemory()).ToArray());
        }
    }
}