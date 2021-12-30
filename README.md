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
- Customize node looking by each properties via [`INodeProperty`](Packages/com.quabug.graph-ext/NodeProperties)
- Have [`Memory`](Packages/com.quabug.graph-ext/Backend/Memory), [`ScriptableObject`](Packages/com.quabug.graph-ext/Backend/ScriptableObject) and [`Prefab`](Packages/com.quabug.graph-ext/Backend/Prefab) back-end to store graph data by default.

## Architecture
```
    +----------------------------------------------------------------------+
    |                                                                      |
    |  Editor                                                              |
    |                                                                      |
    |            +-------------------------------------------+             |
    |            |                 GraphView                 |             |
    |            +-------------------------------------------+             |
    |            |                                           |             |
    |            | 1. Implement `UIElement.Graph`            |             |
    |            | 2. Pull graph data from `GraphViewModule` |             |
    |            |    and generate corresponding elements    |             |
    |            | 3. Send data/event to back-end            |             |
    |            |    via API of `GraphViewModule`           |             |
    |            |                                           |             |
    |            +---------------------+---------------------+             |
    |                                  |                                   |
    |                                  |                                   |
    |                                  v                                   |
    |               +------------------+-------------------+               |
    |               |           GraphViewModule            |               |
    |               +--------------------------------------+               |
    |               |1. Data collection of View            |               |
    |               |   Including nodes, ports and edges.  |               |
    |               |2. Provide API to `GraphView`         |               |
    |               |   to control back-end                |               |
    |               +--------------------------------------+               |
    |                                  ^                                   |
    |                                  | inhenrited                        |
    |                                  |                                   |
    |          +-----------------------+-----------------------+           |
    |          |             BackendGraphViewModule*           |           |
    |          +-----------------------------------------------+           |
    |          |  1. Implement `GraphViewModule`               |           |
    |          |  2. Collect data from specific back-end       |           |
    |          |  3. Bridge call from `GraphView` to back-end  |           |
    |          +--+----------------------------+---------------+           |
    |             |                            |                           |
    |             |                            |                           |
    |             |                            |                           |
    +----------------------------------------------------------------------+
                  |                            |
                  |                            |
+-------------------------------------------------------------------------------+
|                 |                            |                                |
|   Runtime       |                            |                                |
|                 |                            v                                |
|                 |         +------------------+-------------------------+      |
|                 |         |            GraphRuntime<TNode>             |      |
|                 |         +--------------------------------------------+      |
|                 |         |Runtime graph data including nodes and edges|      |
|                 |         +-------------+------------------------------+      |
|                 |                       ^                                     |
|                 |                       |                                     |
|                 |                       |                                     |
|                 v                       |                                     |
|     +-----------+-----------------------+-----+                               |
|     |           BackendRuntime*               |                               |
|     +-----------------------------------------+                               |
|     |Data and processing for specifc back-end,|                               |
|     |like prefab, ScriptableObject, etc.      |                               |
|     +-----------------------------------------+                               |
|                                                                               |
|                                                                               |
+-------------------------------------------------------------------------------+

```
* [`Memory`](Packages/com.quabug.graph-ext/Backend/Memory), [`ScriptableObject`](Packages/com.quabug.graph-ext/Backend/ScriptableObject) and [`Prefab`](Packages/com.quabug.graph-ext/Backend/Prefab) back-end by default.

## Tutorial (Binary Expression Tree)

Step-by-step tutorial to build a following binary expression tree:
![image](https://user-images.githubusercontent.com/683655/147669435-5c057c71-ad60-4f01-a177-bece9091b912.png)

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
[NodeTitle(ConstTitle = "add")] // set node title
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

### 3. Define graph menu entries and window extension to use `ExpressionGraphScriptableObject` as backend:
``` c#
public class ExpressionNodeCreationMenuEntry : ScriptableObjectNodeCreationMenuEntry<IExpressionNode, ExpressionNodeScriptableObject> {}
public class ExpressionNodeWindowExtension : ScriptableObjectWindowExtension<IExpressionNode, ExpressionNodeScriptableObject> {}
```

### 4. Create a new graph window config file and set to use expression extensions:
![image](https://user-images.githubusercontent.com/683655/147674011-a0a5b768-2cd7-4f65-91bf-9a064ebe393c.png)
![image](https://user-images.githubusercontent.com/683655/147674159-bad96cef-5f31-4505-9305-f607d088b308.png)

### 5. Open expression graph window and choose a "Expression Graph" to modifying:
![image](https://user-images.githubusercontent.com/683655/147678233-2d4155cf-d46c-45ca-bf6a-d33925d87ba5.png)
![image](https://user-images.githubusercontent.com/683655/147678357-a8385a57-5070-4404-9a65-d1c1077bb9d5.png)

### 6. (Optional) Make it nicer:
- compact node look of `AddNode`:![image](https://user-images.githubusercontent.com/683655/147679433-baf816ef-d6ab-475c-9388-1396870f1191.png)
``` c#
// delete `[NodeTitle]` to hide title
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
    
    2. Define necessary component, menu entry and window extension:
    ``` c#
    // ExpressionTreeNodeComponent.cs
    public class ExpressionTreeNodeComponent : TreeNodeComponent<IExpressionNode, ExpressionTreeNodeComponent> {}
    public class PrefabExpressionNodeCreationMenuEntry : NodeCreationMenuEntry<IExpressionNode, ExpressionTreeNodeComponent> {}
    public class PrefabExpressionWindowExtension : PrefabStageWindowExtension<IExpressionNode, ExpressionTreeNodeComponent> {}
    ```
    
    3. Create window config:

    ![image](https://user-images.githubusercontent.com/683655/147681310-dbe5261f-dea6-4889-9db3-546aecf51ed8.png)
    
    4. Open a prefab and start to modify:
 
    ![image](https://user-images.githubusercontent.com/683655/147681512-e63c90b3-6727-4353-8434-a9179c6cdf62.png)
    
## HowTo
### Customize graph menu
1. Implement a class of [`IMenuEntry`](Packages/com.quabug.graph-ext/Core/Editor/Menu/IMenuEntry.cs)
2. Add it into _Menu_ of `GraphConfig`

![image](https://user-images.githubusercontent.com/683655/147736001-46d230c6-60ca-452e-a258-44e8380f894e.png)

### Customize view of node property
1. Implement a property factory of [`INodePropertyFactory`](Packages/com.quabug.graph-ext/Core/Editor/View/NodePropertyViewFactory.cs)
``` c#
    public class FloatFieldViewFactory : INodePropertyViewFactory
    {
        public VisualElement Create(Node node, INodeProperty property, INodePropertyViewFactory factory)
        {
            return property is FieldInfoProperty<float> _ ? new Label("replace view") : null;
        }
    }
```

2. Add it into the top of _Factories_ of _Node Property View Factory_

![image](https://user-images.githubusercontent.com/683655/147737968-21151171-6b3f-4e3e-9ef1-97d4d23374d3.png)

3. Create a node and will change its property view.

![image](https://user-images.githubusercontent.com/683655/147737633-a7c982f0-1e86-4e34-8ca8-d89025014ece.png)

