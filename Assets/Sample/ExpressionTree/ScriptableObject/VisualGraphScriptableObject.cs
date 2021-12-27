using GraphExt;
using UnityEngine;

[CreateAssetMenu(fileName = "VisualGraph", menuName = "Graph/Graph Root (VisualNode)", order = 0)]
public class VisualGraphScriptableObject : GraphScriptableObject<IVisualNode, VisualNodeScriptableObject> {}