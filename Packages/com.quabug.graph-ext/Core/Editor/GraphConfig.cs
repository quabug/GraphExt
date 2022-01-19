using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    [CreateAssetMenu(fileName = "Graph Window Config", menuName = "Graph/New Window Config", order = 0)]
    public class GraphConfig : ScriptableObject
    {
        public string WindowName = "Graph Window";
        [SerializedType(typeof(BaseGraphWindow), InstantializableType = true, Nullable = false)] public string WindowType;

        [SerializeReference, SerializeReferenceDrawer] public INodeViewFactory NodeViewFactory = new DefaultNodeViewFactory();
        [SerializeReference, SerializeReferenceDrawer] public IPortViewFactory PortViewFactory = new DefaultPortViewFactory();
        [SerializeReference, SerializeReferenceDrawer] public IEdgeViewFactory EdgeViewFactory = new DefaultEdgeViewFactory();

        public StyleSheet WindowStyleSheet;

        private void Reset()
        {
            NodeViewFactory = new DefaultNodeViewFactory();
            PortViewFactory = new DefaultPortViewFactory();
            EdgeViewFactory = new DefaultEdgeViewFactory();
        }

        [ContextMenu("Open Window")]
        public void OpenWindow()
        {
            var window = (BaseGraphWindow) Window.GetOrCreate(Type.GetType(WindowType), WindowName);
            window.titleContent.text = WindowName;
            window.Show(immediateDisplay: true);
            window.Focus();
            window.Config = this;
        }
    }
}