using System;
using System.Collections.Generic;
using OneShot;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    [Serializable]
    public class StickyNoteInstaller<TStickyNoteSystem> : IInstaller
        where TStickyNoteSystem : StickyNoteSystem
    {
        [SerializedType(typeof(IStickyNoteViewFactory), Nullable = false, InstantializableType = true, RenamePatter = @"\w*\.||")]
        public string ViewFactory;

        public void Install(Container container)
        {
            container.RegisterTypeNameSingleton<IStickyNoteViewFactory>(ViewFactory);
            container.RegisterSingleton<TStickyNoteSystem>();
            container.Register<StickyNoteSystem>(container.Resolve<TStickyNoteSystem>);
            container.RegisterBiDictionaryInstance(new BiDictionary<StickyNoteId, StickyNote>());
            container.RegisterDictionaryInstance(new Dictionary<StickyNoteId, StickyNoteData>());
            container.RegisterSingleton<StickyNotePresenter>();
            container.Register<IViewPresenter>(container.Resolve<StickyNotePresenter>);
            container.Register<AddNote>(() => container.Resolve<TStickyNoteSystem>().AddNote);
            container.Register<RemoveNoteView>(() => container.Resolve<TStickyNoteSystem>().RemoveNote);
        }
    }

    public class MemoryStickyNoteInstaller : StickyNoteInstaller<MemoryStickyNoteSystem> {}
    public class PrefabStickyNoteInstaller : StickyNoteInstaller<PrefabStickyNoteSystem> {}
    public class ScriptableObjectStickyNoteInstaller : StickyNoteInstaller<ScriptableObjectStickyNoteSystem> {}
}