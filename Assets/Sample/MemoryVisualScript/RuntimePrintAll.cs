using System.Linq;
using UnityEngine;

public class RuntimePrintAll : MonoBehaviour
{
    public TextAsset GraphJson;

    private void Awake()
    {
        var graph = JsonUtility.Load(GraphJson.text);
        foreach (var node in graph.Nodes.OfType<IVisualNode>())
            Debug.Log($"{node.GetType().Name} = {node.GetValue(graph)}");
    }
}