using System;
using OneShot;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    [Serializable]
    public class StickyNoteInstaller<TStickyNoteSystem> : IInstaller
        where TStickyNoteSystem : StickyNoteSystem
    {
        [SerializeReference, SerializeReferenceDrawer(Nullable = false)]
        public IStickyNoteViewFactory ViewFactory;

        public void Install(Container container)
        {
            container.RegisterSingleton<TStickyNoteSystem>();
            container.RegisterInstance(ViewFactory);
            container.RegisterBiDictionaryInstance(new BiDictionary<StickyNoteId, StickyNote>());
            container.RegisterSingleton<StickyNotePresenter>();
            container.Register<AddNote>(() => container.Resolve<StickyNoteSystem>().AddNote);
            container.Register<RemoveNoteView>(() => container.Resolve<StickyNoteSystem>().RemoveNote);
        }
    }
}