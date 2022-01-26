using System;
using System.Collections.Generic;
using OneShot;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    [Serializable]
    public class StickyNoteGraphInstaller<TStickyNoteSystem> : IGraphInstaller
        where TStickyNoteSystem : StickyNoteSystem
    {
        [SerializedType(typeof(IStickyNoteViewFactory), Nullable = false, InstantializableType = true, RenamePatter = @"\w*\.||")]
        public string ViewFactory = typeof(DefaultStickyNoteViewFactory).AssemblyQualifiedName;

        public void Install(Container container, TypeContainers typeContainers)
        {
            container.RegisterTypeNameSingleton<IStickyNoteViewFactory>(ViewFactory);
            container.RegisterSingleton<TStickyNoteSystem>();
            container.Register<StickyNoteSystem>(container.Resolve<TStickyNoteSystem>);
            container.RegisterBiDictionaryInstance(new BiDictionary<StickyNoteId, StickyNote>());
            container.RegisterDictionaryInstance(new Dictionary<StickyNoteId, StickyNoteData>());
            container.RegisterSingleton<StickyNotePresenter>();
            container.Register<IWindowSystem>(container.Resolve<StickyNotePresenter>);
            container.Register<AddNote>(() => container.Resolve<TStickyNoteSystem>().AddNote);
            container.Register<RemoveNoteView>(() => container.Resolve<TStickyNoteSystem>().RemoveNote);
        }
    }

    public class MemoryStickyNoteGraphInstaller : StickyNoteGraphInstaller<MemoryStickyNoteSystem> {}
    public class PrefabStickyNoteGraphInstaller : StickyNoteGraphInstaller<PrefabStickyNoteSystem> {}
    public class ScriptableObjectStickyNoteGraphInstaller : StickyNoteGraphInstaller<ScriptableObjectStickyNoteSystem> {}
}