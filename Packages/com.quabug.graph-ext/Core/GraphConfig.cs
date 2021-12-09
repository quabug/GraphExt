using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt
{
    [CreateAssetMenu(fileName = "GraphConfig", menuName = "Graph/New Config", order = 0)]
    public class GraphConfig : ScriptableObject
    {
        public string WindowName = "Graph Window";
        [SerializedType(typeof(IGraphModule))] public string Backend;
        [SerializeReference, SerializeReferenceDrawer] public IMenuEntry[] Menu;
        [SerializeReference, SerializeReferenceDrawer] public INodePropertyViewFactory[] NodePropertyViewFactories;

        [ContextMenu("Open Window")]
        public void OpenWindow()
        {
            var window = Window.GetOrCreate<GraphWindow>(WindowName);
            window.titleContent.text = WindowName;
            window.Init(this);
            window.Focus();
        }

        [CanBeNull] public VisualElement CreatePropertyView([NotNull] INodeProperty property)
        {
            return NodePropertyViewFactories.Select(factory => factory.Create(property)).FirstOrDefault(view => view != null);
        }
    }
}