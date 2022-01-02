namespace GraphExt
{
    public interface ITreeNode<in TGraph> : INode<TGraph>
    {
        string InputPortName { get; }
        string OutputPortName { get; }
    }
}