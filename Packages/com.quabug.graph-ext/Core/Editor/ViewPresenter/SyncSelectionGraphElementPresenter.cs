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
        public delegate Object ConvertGraphSelectableToObject(ISelectable selectable);
        public delegate ISelectable ConvertObjectToGraphSelectable(Object @object);

        private readonly ConvertGraphSelectableToObject _convertGraphSelectableToObject;
        private readonly ConvertObjectToGraphSelectable _convertObjectToGraphSelectable;

        private readonly UnityEditor.Experimental.GraphView.GraphView _graphView;

        private HashSet<Object> _selectedObjectsCache = new HashSet<Object>();
        private HashSet<ISelectable> _selectedElementsCache = new HashSet<ISelectable>();

        public SyncSelectionGraphElementPresenter(
            UnityEditor.Experimental.GraphView.GraphView graphView,
            ConvertGraphSelectableToObject convertGraphSelectableToObject,
            ConvertObjectToGraphSelectable convertObjectToGraphSelectable
        )
        {
            _graphView = graphView;
            _convertGraphSelectableToObject = convertGraphSelectableToObject;
            _convertObjectToGraphSelectable = convertObjectToGraphSelectable;
            Selection.selectionChanged += SyncObjectToGraph;
        }

        void SyncObjectToGraph()
        {
            var selectedElements = new HashSet<ISelectable>();
            _selectedObjectsCache.Clear();
            foreach (var obj in Selection.objects)
            {
                var selectable = _convertObjectToGraphSelectable(obj);
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

        public void Dispose()
        {
            Selection.selectionChanged -= SyncObjectToGraph;
        }

        public void Tick()
        {
            SyncGraphToObject();
        }

        private void SyncGraphToObject()
        {
            var selectedObjects = new HashSet<Object>();
            _selectedElementsCache.Clear();
            foreach (var seletable in _graphView.selection)
            {
                var obj = _convertGraphSelectableToObject(seletable);
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