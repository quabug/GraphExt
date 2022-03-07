namespace GraphExt.Editor
{
    public class TypeNameMenuEntriesInstaller : IMenuEntryInstaller
    {
        [SerializedType(typeof(IMenuEntry), Nullable = false, InstantializableType = true, RenamePatter = @"\w*\.||")]
        public string[] MenuEntries;

        public void Install(Container container)
        {
            container.RegisterTypeNameArraySingleton<IMenuEntry>(MenuEntries);
        }
    }
}