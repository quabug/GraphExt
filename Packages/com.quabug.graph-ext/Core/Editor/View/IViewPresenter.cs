using System;

namespace GraphExt.Editor
{
    public interface IViewPresenter : IDisposable
    {
        void Tick();
    }
}