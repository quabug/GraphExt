using System;

namespace GraphExt
{
    public interface INodeView : IDisposable
    {
        int Id { get; }
        void SyncPosition();
    }
}