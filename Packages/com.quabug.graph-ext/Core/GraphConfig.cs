using System;
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
        [SerializeReference, SerializeReferenceDrawer(Nullable = false)]
        public INodePropertyViewFactory NodePropertyViewFactory;

#if UNITY_EDITOR
        private void Reset()
        {
            Menu = UnityEditor.TypeCache
                .GetTypesDerivedFrom<IMenuEntry>()
                .Where(type => !type.IsAbstract && !type.IsGenericType && type.GetConstructor(Array.Empty<Type>()) != null)
                .Select(type => (IMenuEntry)Activator.CreateInstance(type))
                .ToArray()
            ;

            NodePropertyViewFactory = new OrderedGroupNodePropertyViewFactory
            {
                Factories = UnityEditor.TypeCache.GetTypesDerivedFrom<INodePropertyViewFactory>()
                    .Where(type => !type.IsAbstract && !type.IsGenericType && type.GetConstructor(Array.Empty<Type>()) != null)
                    .Where(type => type != typeof(OrderedGroupNodePropertyViewFactory) &&
                                   type != typeof(INodePropertyViewFactory.Null) &&
                                   type != typeof(INodePropertyViewFactory.Exception)
                                   )
                    .Select(type => (INodePropertyViewFactory) Activator.CreateInstance(type))
                    .ToArray()
            };
        }
#endif

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
            return NodePropertyViewFactory.Create(property, new INodePropertyViewFactory.Null());
        }
    }
}