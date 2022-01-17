using UnityEngine;

public class RuntimePrintAll : MonoBehaviour
{
    public TextAsset GraphJson;

    private void Awake()
    {
        var graph = JsonUtility.Deserialize<IVisualNode>(GraphJson.text);
        foreach (var node in graph.NodeMap.Values)
            Debug.Log($"{node.GetType().Name} = {node.GetValue(graph)}");
    }
}