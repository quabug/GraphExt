using System;
using System.IO;
using System.Linq;
using GraphExt;
using GraphExt.Memory;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MemorySaveLoadMenu : IMenuEntry
{
    private JsonSerializerSettings _jsonSerializerSettings;

    public MemorySaveLoadMenu()
    {
        _jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
    }

    public void MakeEntry(GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
    {
        if (graph.Module is Graph memoryGraph)
        {
            menu.AddItem(new GUIContent("Save"), false, () => Save(memoryGraph));
            menu.AddItem(new GUIContent("Load"), false, () =>
            {
                var newGraph = Load();
                if (newGraph != null) graph.Module = newGraph;
            });
        }
    }

    private void Save(Graph graph)
    {
        var path = EditorUtility.SaveFilePanel("save path", Application.dataPath, "graph", "json");
        if (string.IsNullOrEmpty(path)) return;
        try
        {
            var json = JsonConvert.SerializeObject(new SerializableGraph(graph), Formatting.Indented, _jsonSerializerSettings);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to save graph to {path}: {ex}");
        }
    }

    private Graph Load()
    {
        var path = EditorUtility.OpenFilePanel("load graph", Application.dataPath, "json");
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