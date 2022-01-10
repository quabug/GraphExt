using System.Collections.Generic;
using GraphExt.Editor;
using UnityEngine;

namespace GraphExt
{
    public class ScriptableObjectNodePositions : IViewModuleElements<NodeId, Vector2>
    {


        public Vector2 this[in NodeId id]
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        public bool Has(in NodeId id)
        {
            throw new System.NotImplementedException();
        }

        public bool Has(Vector2 data)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<NodeId> Ids { get; }
        public IEnumerable<Vector2> Datas { get; }
        public IEnumerable<(NodeId id, Vector2 data)> Elements { get; }
        public void Add(in NodeId id, Vector2 data)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(in NodeId id)
        {
            throw new System.NotImplementedException();
        }
    }
}