using System;

namespace GraphExt
{
    public interface ISelectable
    {
        bool IsSelected { set; }
        event Action OnSelected;
    }
}