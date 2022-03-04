namespace GraphExt.Editor
{
    public class TypeNameMenuEntryInstaller : IMenuEntryInstaller
    {
        [SerializedType(typeof(IMenuEntry), Nullable = false, InstantializableType = true, RenamePatter = @"\w*\.||")]
        public string MenuEntryType;

        public void Install(Container container)
        {
            container.RegisterTypeNameSingleton<IMenuEntry>(MenuEntryType);
        }
    }
}