using System;
using System.Collections.Generic;
using OneShot;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
            container.RegisterSingleton<TStickyNoteSystem>();
            container.Register<StickyNoteSystem<TStickyNoteData>>(container.Resolve<TStickyNoteSystem>);
            container.Register<IReadOnlyBiDictionary<StickyNoteId, TStickyNoteData>>(() => container.Resolve<TStickyNoteSystem>().StickyNotes);
            container.RegisterBiDictionaryInstance(new BiDictionary<StickyNoteId, StickyNote>());
            container.RegisterDictionaryInstance(new Dictionary<StickyNoteId, StickyNoteData>());
            container.RegisterSingleton<StickyNotePresenter>();
            container.Register<IWindowSystem>(container.Resolve<StickyNotePresenter>);
            container.Register<AddNote>(() => container.Resolve<TStickyNoteSystem>().AddNote);
            container.Register<RemoveNoteView>(() => container.Resolve<TStickyNoteSystem>().RemoveNote);
        }
    }

    public class MemoryStickyNoteGraphInstaller : StickyNoteGraphInstaller<MemoryStickyNoteSystem, StickyNoteData> {}

    public class PrefabStickyNoteGraphInstaller : StickyNoteGraphInstaller<PrefabStickyNoteSystem, StickyNoteComponent>
    {
        public override void Install(Container container, TypeContainers typeContainers)
        {
            base.Install(container, typeContainers);
            container.Register<IWindowSystem>(() =>
            {
                var graphView = container.Resolve<UnityEditor.Experimental.GraphView.GraphView>();
                var noteViews = container.Resolve<IReadOnlyBiDictionary<StickyNoteId, StickyNote>>();
                var notes = container.Resolve<IReadOnlyBiDictionary<StickyNoteId, StickyNoteComponent>>();
                return new SyncSelectionGraphElementPresenter(
                    graphView,
                    selectable => selectable is StickyNote note ? notes[noteViews.Reverse[note]].gameObject : null,
                    obj =>
                    {
                        var noteComponent = obj is GameObject note ? note.GetComponent<StickyNoteComponent>() : null;
                        return noteComponent == null ? null : noteViews[notes.Reverse[noteComponent]];
                    });
            });
        }
    }

    public class ScriptableObjectStickyNoteGraphInstaller : StickyNoteGraphInstaller<ScriptableObjectStickyNoteSystem, StickyNoteScriptableObject> {}
}