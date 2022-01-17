using GraphExt;
using UnityEngine;

public class PrefabPrintAll : MonoBehaviour
{
    public GameObject Root;

    private void Awake()
    {
        var nodes = new GameObjectNodes<IVisualNode, VisualTreeComponent>(Root);
        foreach (var pair in nodes.Runtime.NodeMap)
        {
            var node = pair.Value;
            Debug.Log($"{node.GetType().Name} = {node.GetValue(nodes.Runtime)}");
        }
    }
}
