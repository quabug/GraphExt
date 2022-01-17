> ⚠️ This is not a comprehensive Visual Script solution.
# GraphExt
A library to help you customize your own Unity3D Graph solution.

## Why not use `UIElement.Graph` directly?
1. Editor-only, you have to implement your own runtime graph if needed.
2. Lack of serializer and deserializer.
3. Not easy to start, have to set up bunch of stuff before actually write your own code.

## Features
- Separate runtime and editor graph.
- Easy to scratch a new type of node from [`INode`](Packages/com.quabug.graph-ext/Core/Runtime/Data/INode.cs)
- Customize node by each properties via [`INodeProperty`](Packages/com.quabug.graph-ext/NodeProperties)
- Customize graph menu by extend [`IMenuEntry`](Packages/com.quabug.graph-ext/Core/Editor/Menu/IMenuEntry.cs)
- Have [`Memory`](Packages/com.quabug.graph-ext/Backend/Memory), [`ScriptableObject`](Packages/com.quabug.graph-ext/Backend/ScriptableObject) and [`Prefab`](Packages/com.quabug.graph-ext/Backend/Prefab) back-end to store graph data by default.

## Architecture
![image](https://user-images.githubusercontent.com/683655/149826813-7fa740a1-0195-41b1-b538-9626563934e6.png)

## Tutorial (Binary Expression Tree)
<details><summary>Step-by-step tutorial to build a following binary expression tree:
    
![image](https://user-images.githubusercontent.com/683655/147669435-5c057c71-ad60-4f01-a177-bece9091b912.png)
</summary>

### 1. Define runtime nodes:
1. Define the interface of expression node:
``` c#
public interface IExpressionNode : INode<GraphRuntime<IExpressionNode>>
{
    float GetValue([NotNull] GraphRuntime<IExpressionNode> graph);
}
```

2. Define `ValueNode` with a single-input-port and a value property.
``` c#
[Serializable]
public class ValueNode : IExpressionNode
{
    // define a single-float-input port
    [NodePort] private static float _input;
    // define a node property of `_value:SerializedProperty`
    [NodeProperty(InputPort = nameof(_input))] public float Value;

    public float GetValue(GraphRuntime<IExpressionNode> _)
    {
        return Value;
    }

    public bool IsPortCompatible(GraphRuntime<IExpressionNode> graph, in PortId input, in PortId output) => true;
    public void OnConnected(GraphRuntime<IExpressionNode> graph, in PortId input, in PortId output) {}
    public void OnDisconnected(GraphRuntime<IExpressionNode> graph, in PortId input, in PortId output) {}
}
```

3. Define `AddNode` with a single-input-port and a multi-output-port. `GetValue` will sum the value of output-connected nodes together.
``` c#
[Serializable]
public class AddNode : IExpressionNode
{
    // define a single-float-input port
    [NodePort(Direction = PortDirection.Input, HideLabel = true)] private static float _input;
    // define a multi-float-output port with at most 2 connections
    [NodePort(Direction = PortDirection.Output, Capacity = 2, HideLabel = true)] private static float _output;

    public float GetValue(GraphRuntime<IExpressionNode> graph)
    {
        var thisNodeId = graph[this];
        var inputPortId = new PortId(thisNodeId, nameof(_input));
        var connectedNodeIds = graph.FindConnectedNodes(inputPortId);
        var connectedNodes = connectedNodeIds.Select(nodeId => graph[nodeId]);
        return connectedNodes.Sum(node => node.GetValue(graph));
    }

    public bool IsPortCompatible(GraphRuntime<IExpressionNode> graph, in PortId input, in PortId output) => true;
    public void OnConnected(GraphRuntime<IExpressionNode> graph, in PortId input, in PortId output) {}
    public void OnDisconnected(GraphRuntime<IExpressionNode> graph, in PortId input, in PortId output) {}
}
```

### 2. Define graph and node `ScriptableObject` to store graph data.
``` c#
// ExpressionNodeScriptableObject.cs
public class ExpressionNodeScriptableObject : NodeScriptableObject<IExpressionNode> {}
```
``` c#
// ExpressionGraphScriptableObject.cs
[CreateAssetMenu(menuName = "Expression Graph", fileName = "Graph/New Expression Graph", order = 0)]
public class ExpressionGraphScriptableObject : GraphScriptableObject<IExpressionNode, ExpressionNodeScriptableObject> {}
```
![image](https://user-images.githubusercontent.com/683655/147674310-bdeae924-014a-4dd9-a5a1-ae6effd8644e.png)

### 3. Create a new graph window of scriptable object backend:
``` C#
public class ScriptableObjectExpressionTreeWindow : ScriptableObjectGraphWindow<IVisualNode, VisualNodeScriptableObject>
{
    private MenuBuilder _menuBuilder;

    [MenuItem("Graph/ScriptableObject Expression Tree")]
    public static void OpenWindow()
    {
        OpenWindow<ScriptableObjectExpressionTreeWindow>("Scriptable Object");
    }

    protected override void CreateMenu()
    {
        _menuBuilder = new MenuBuilder(_GraphSetup.GraphView, new IMenuEntry[]
        {
            new PrintValueMenu(_GraphSetup.GraphRuntime, _GraphSetup.NodeViews.Reverse),
            new SelectionEntry<IVisualNode>(_GraphSetup.GraphRuntime, _GraphSetup.NodeViews.Reverse, _GraphSetup.EdgeViews.Reverse),
            new NodeMenuEntry<IVisualNode>(_GraphSetup.GraphRuntime, _GraphSetup.NodePositions)
        });
    }
}
```

### 4. Open expression graph window and choose a "Expression Graph" to modifying:
![image](https://user-images.githubusercontent.com/683655/149827415-12e86d6f-c933-478f-a24c-de0ead625bfd.png)
![image](https://user-images.githubusercontent.com/683655/147678357-a8385a57-5070-4404-9a65-d1c1077bb9d5.png)

### 5. (Optional) Make it nicer:
- compact node look of `AddNode`:![image](https://user-images.githubusercontent.com/683655/147679433-baf816ef-d6ab-475c-9388-1396870f1191.png)
``` c#
public class AddNode : IExpressionNode
{
    // define a property to hold input and output port
    [NodeProperty(HideValue = true, InputPort = nameof(_input), OutputPort = nameof(_output))] private static int add;
    ...
```

- make a base expression node:
``` c#
public abstract class ExpressionNode : IExpressionNode
{
    [NodePort(Direction = PortDirection.Input, Capacity = 1, Hide = true)] protected static float _input;
    [NodePort(Direction = PortDirection.Output, Capacity = 2, Hide = true)] protected static float _output;

    public bool IsPortCompatible(GraphRuntime<IExpressionNode> graph, in PortId input, in PortId output) => true;
    public void OnConnected(GraphRuntime<IExpressionNode> graph, in PortId input, in PortId output) {}
    public void OnDisconnected(GraphRuntime<IExpressionNode> graph, in PortId input, in PortId output) {}

    public abstract float GetValue(GraphRuntime<IExpressionNode> graph);

    protected IEnumerable<IExpressionNode> GetConnectedNodes(GraphRuntime<IExpressionNode> graph)
    {
        var thisNodeId = graph[this];
        var inputPortId = new PortId(thisNodeId, nameof(_input));
        var connectedNodeIds = graph.FindConnectedNodes(inputPortId);
        var connectedNodes = connectedNodeIds.Select(nodeId => graph[nodeId]);
        return connectedNodes;
    }
}

[Serializable]
public class ValueNode : ExpressionNode
{
    [NodeProperty(InputPort = nameof(_input))] public float Value;
    public override float GetValue(GraphRuntime<IExpressionNode> _) => Value;
}

[Serializable]
public class AddNode : ExpressionNode
{
    [NodeProperty(HideValue = true, InputPort = nameof(_input), OutputPort = nameof(_output))] private static int add;
    public override float GetValue(GraphRuntime<IExpressionNode> graph) => GetConnectedNodes(graph).Sum(node => node.GetValue(graph));
}
```

- Use prefab (and GameObject tree) as backend:
    1. Use `ITreeNode<>` interface instead of `INode<>`
    ``` c#
    public interface IExpressionNode : ITreeNode<GraphRuntime<IExpressionNode>>
    {
        float GetValue([NotNull] GraphRuntime<IExpressionNode> graph);
    }

    public abstract class ExpressionNode : IExpressionNode
    {
        public string InputPortName => nameof(_input);
        public string OutputPortName => nameof(_output);
    ...
    ```
    
    2. Define tree component:
    ``` c#
    // ExpressionTreeNodeComponent.cs
    public class ExpressionTreeNodeComponent : TreeNodeComponent<IExpressionNode, ExpressionTreeNodeComponent> {}
    ```
    
    3. Create prefab window:

    ``` c#
    public class PrefabExpressionTreeWindow : PrefabGraphWindow<IVisualNode, VisualTreeComponent>
    {
        private MenuBuilder _menuBuilder;

        [MenuItem("Graph/Prefab Expression Tree")]
        public static void OpenWindow()
        {
            OpenWindow<PrefabExpressionTreeWindow>("Prefab");
        }

        protected override void CreateMenu()
        {
            _menuBuilder = new MenuBuilder(_GraphSetup.GraphView, new IMenuEntry[]
            {
                new PrintValueMenu(_GraphSetup.GraphRuntime, _GraphSetup.NodeViews.Reverse),
                new SelectionEntry<IVisualNode>(_GraphSetup.GraphRuntime, _GraphSetup.NodeViews.Reverse, _GraphSetup.EdgeViews.Reverse),
                new NodeMenuEntry<IVisualNode>(_GraphSetup.GraphRuntime, _GraphSetup.NodePositions)
            });
        }
    }
    ```
    
    4. Open a prefab and start to modify:
 
    ![image](https://user-images.githubusercontent.com/683655/147681512-e63c90b3-6727-4353-8434-a9179c6cdf62.png)
    
</details>
