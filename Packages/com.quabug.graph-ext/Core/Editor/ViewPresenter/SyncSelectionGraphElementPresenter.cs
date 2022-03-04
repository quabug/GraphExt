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
        private readonly UnityEditor.Experimental.GraphView.GraphView _graphView;
        private readonly Object _root;
        private readonly System[] _systems;

        public SyncSelectionGraphElementPresenter(
            UnityEditor.Experimental.GraphView.GraphView graphView,
            Func<IConvertor, System> systemFactory,
            IConvertor[] convertors = null,
            Object root = null
        )
        {
            _graphView = graphView;
            _root = root;
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

            var hasSelectedObjects = _systems.Any(system => system.SelectedObjectsCache.Any());
            if (!hasSelectedObjects && _root != null && (Selection.objects == null || !Selection.objects.Any())) Selection.objects = new[] { _root };
            else if (hasSelectedObjects && _root != null) Selection.objects = Selection.objects.Where(obj => obj != _root).ToArray();
        }

        private void OnSelectionChanged()
        {
            var changed = false;
            foreach (var system in _systems) changed = system.SyncObjectToGraph() || changed;
            if (changed) _graphView.FrameSelection();
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

            public bool SyncObjectToGraph()
            {
                var selectedElements = new HashSet<ISelectable>();
                _selectedObjectsCache.Clear();
                foreach (var obj in Selection.objects)
                {
                    var selectable = _convertor.ConvertObjectToGraphSelectable(obj);
                    if (selectable != null)
                    {
                        selectedElements.Add(selectable);
                        _selectedObjectsCache.Add(obj);
                    }
                }
                var (addedElements, deletedElements) = _selectedElementsCache.Diff(selectedElements);
                if (!addedElements.Any() && !deletedElements.Any()) return false;

                _selectedElementsCache = selectedElements;
                foreach (var added in addedElements) added.Select(_graphView, true);
                foreach (var deleted in deletedElements) deleted.Unselect(_graphView);
                return true;
            }

            public bool SyncGraphToObject()
            {
                var selectedObjects = new HashSet<Object>();
                _selectedElementsCache.Clear();
                foreach (var selectable in _graphView.selection)
                {
                    var obj = _convertor.ConvertGraphSelectableToObject(selectable);
                    if (obj != null)
                    {
                        selectedObjects.Add(obj);
                        _selectedElementsCache.Add(selectable);
                    }
                }
                var (addedObjects, deletedObjects) = _selectedObjectsCache.Diff(selectedObjects);
                if (!addedObjects.Any() && !deletedObjects.Any()) return false;

                _selectedObjectsCache = selectedObjects;
                var objects = new HashSet<Object>(Selection.objects);
                objects.ExceptWith(deletedObjects);
                objects.UnionWith(addedObjects);
                Selection.objects = objects.ToArray();
                return true;
            }
        }
    }
}