using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Object = UnityEngine.Object;

namespace GraphExt.Editor
{
    /// <summary>
    /// Make `GraphView` focus on selected object of `Selection`
    /// </summary>
    public class SyncSelectionGraphElementPresenter : IDisposable, ITickableWindowSystem
    {
        private readonly System[] _systems;

        public SyncSelectionGraphElementPresenter(Func<IConvertor, System> systemFactory, IConvertor[] convertors = null)
        {
            _systems = convertors == null ? Array.Empty<System>() : convertors.Select(systemFactory).ToArray();
            Selection.selectionChanged += OnSelectionChanged;
        }

        public void Dispose()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        public void Tick()
        {
            foreach (var system in _systems) system.SyncGraphToObject();
        }

        private void OnSelectionChanged()
        {
            foreach (var system in _systems) system.SyncObjectToGraph();
        }

        public interface IConvertor
        {
            Object ConvertGraphSelectableToObject(ISelectable selectable);
            ISelectable ConvertObjectToGraphSelectable(Object @object);
        }

        public class System
        {
            private readonly UnityEditor.Experimental.GraphView.GraphView _graphView;
            private readonly IConvertor _convertor;

            private HashSet<Object> _selectedObjectsCache = new HashSet<Object>();
            public IReadOnlySet<Object> SelectedObjectsCache => _selectedObjectsCache;

            private HashSet<ISelectable> _selectedElementsCache = new HashSet<ISelectable>();
            public IReadOnlySet<ISelectable> SelectedElementsCache => _selectedElementsCache;

            public System(UnityEditor.Experimental.GraphView.GraphView graphView, IConvertor convertor)
            {
                _graphView = graphView;
                _convertor = convertor;
            }

            public void SyncObjectToGraph()
            {
                var selectedElements = new HashSet<ISelectable>();
                _selectedObjectsCache.Clear();
                foreach (var obj in Selection.objects)
                {
                    var selectable = _convertor.ConvertObjectToGraphSelectable(obj);
                    if (selectable != null)
                    {
                        _selectedObjectsCache.Clear();
                        selectedElements.Add(selectable);
                    }
                }
                var (addedElements, deletedElements) = _selectedElementsCache.Diff(selectedElements);
                if (!addedElements.Any() && !deletedElements.Any()) return;

                _selectedElementsCache = selectedElements;
                foreach (var added in addedElements) added.Select(_graphView, true);
                foreach (var deleted in deletedElements) deleted.Unselect(_graphView);
            }

            public void SyncGraphToObject()
            {
                var selectedObjects = new HashSet<Object>();
                _selectedElementsCache.Clear();
                foreach (var seletable in _graphView.selection)
                {
                    var obj = _convertor.ConvertGraphSelectableToObject(seletable);
                    if (obj != null)
                    {
                        selectedObjects.Add(obj);
                        _selectedElementsCache.Add(seletable);
                    }
                }
                var (addedObjects, deletedObjects) = _selectedObjectsCache.Diff(selectedObjects);
                if (!addedObjects.Any() && !deletedObjects.Any()) return;

                _selectedObjectsCache = selectedObjects;
                var objects = new HashSet<Object>(Selection.objects);
                objects.ExceptWith(deletedObjects);
                objects.UnionWith(addedObjects);
                Selection.objects = objects.ToArray();
            }
        }
    }
}