using System;
using System.IO;
using System.Linq;
using GraphExt.Memory;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonSaveLoad
{
    private static JsonSerializerSettings _jsonSerializerSettings;

    static JsonSaveLoad()
    {
        _jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
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

    [Serializable]
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

    [Serializable]
    struct SerializableGraph
    {
        public SerializableNode[] Nodes;

        public SerializableGraph(Graph graph)
        {
            Nodes = graph.NodeList.Select(node => new SerializableNode(node)).ToArray();
        }

        public Graph ToMemory()
        {
            return new Graph(Nodes.Select(node => node.ToMemory()).ToList());
        }
    }
}