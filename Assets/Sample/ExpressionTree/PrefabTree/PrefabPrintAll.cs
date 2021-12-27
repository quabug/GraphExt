using GraphExt;
using UnityEngine;

public class PrefabPrintAll : MonoBehaviour
{
    public GameObject Root;

    private void Awake()
    {
        var nodes = new GameObjectNodes<IVisualNode, VisualTreeComponent>(Root);
        foreach (var pair in nodes.Graph.NodeMap)
        {
            var nodeId = pair.Key;
            var node = pair.Value;
            Debug.Log($"{node.GetType().Name} = {node.GetValue(nodes.Graph)}");
        }
    }
}
