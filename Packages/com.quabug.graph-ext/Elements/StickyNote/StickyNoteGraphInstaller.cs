using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    [Serializable]
    public class StickyNoteGraphInstaller<TStickyNoteSystem, TStickyNoteData> : IGraphInstaller
        where TStickyNoteSystem : StickyNoteSystem<TStickyNoteData>
    {
        [SerializedType(typeof(IStickyNoteViewFactory), Nullable = false, InstantializableType = true, RenamePatter = @"\w*\.||")]
        public string ViewFactory = typeof(DefaultStickyNoteViewFactory).AssemblyQualifiedName;

        public virtual void Install(Container container, TypeContainers typeContainers)
        {
            container.RegisterTypeNameSingleton<IStickyNoteViewFactory>(ViewFactory);
            container.Register<TStickyNoteSystem>().Singleton().AsSelf().As<StickyNoteSystem<TStickyNoteData>>();
            container.Register<IReadOnlyBiDictionary<StickyNoteId, TStickyNoteData>>(
                (resolveContainer, contractType) => container.Resolve<TStickyNoteSystem>().StickyNotes
            ).AsSelf();
            container.RegisterBiDictionaryInstance(new BiDictionary<StickyNoteId, StickyNote>());
            container.RegisterDictionaryInstance(new Dictionary<StickyNoteId, StickyNoteData>());
            container.Register<StickyNotePresenter>().Singleton().AsSelf().As<IWindowSystem>();
            container.Register<AddNote>((resolveContainer, contractType) => container.Resolve<TStickyNoteSystem>().AddNote).AsSelf();
            container.Register<RemoveNoteView>((resolveContainer, contractType) => container.Resolve<TStickyNoteSystem>().RemoveNote).AsSelf();
        }
    }

    public class MemoryStickyNoteGraphInstaller : StickyNoteGraphInstaller<MemoryStickyNoteSystem, StickyNoteData> {}

    public class PrefabStickyNoteGraphInstaller : StickyNoteGraphInstaller<PrefabStickyNoteSystem, StickyNoteComponent>
    {
        public override void Install(Container container, TypeContainers typeContainers)
        {
            base.Install(container, typeContainers);
            typeContainers.GetTypeContainer<SyncSelectionGraphElementPresenter>()
                .Register<PrefabNodeSelectionConvertor<StickyNoteId, StickyNote, StickyNoteComponent>>()
                .Singleton()
                .AsSelf()
                .AsInterfaces()
            ;
        }
    }

    public class ScriptableObjectStickyNoteGraphInstaller : StickyNoteGraphInstaller<ScriptableObjectStickyNoteSystem, StickyNoteScriptableObject>
    {
        public override void Install(Container container, TypeContainers typeContainers)
        {
            base.Install(container, typeContainers);
            typeContainers.GetTypeContainer<SyncSelectionGraphElementPresenter>()
                .Register<ScriptableNodeSelectionConvertor<StickyNoteId, StickyNote, StickyNoteScriptableObject>>()
                .Singleton()
                .AsSelf()
                .AsInterfaces()
            ;
        }
    }
}