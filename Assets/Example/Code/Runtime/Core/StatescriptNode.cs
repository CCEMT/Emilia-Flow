using Emilia.Flow;

namespace Emilia.Statescript
{
    public interface IStatescriptNodeAsset { }

    public abstract class StatescriptNodeAsset<T> : UniversalFlowNodeAsset<T>, IStatescriptNodeAsset where T : FlowNode, new() { }

    public abstract class StatescriptNode<T> : UniversalFlowNode<T> where T : FlowNodeAsset { }
}