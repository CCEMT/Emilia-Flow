using System;
using Emilia.Reference;

namespace Emilia.Flow
{
    public interface IUniversalFlowNodeAsset
    {
        Type nodeType { get; }
    }

    public abstract class UniversalFlowNodeAsset<T> : FlowNodeAsset, IUniversalFlowNodeAsset where T : FlowNode, new()
    {
        public Type nodeType => typeof(T);

        public override FlowNode CreateNode()
        {
            return ReferencePool.Acquire<T>();
        }
    }

    public abstract class UniversalFlowNode<T> : FlowNode where T : FlowNodeAsset
    {
        protected T asset;

        protected override void OnInit()
        {
            this.asset = flowNodeAsset as T;
        }
    }
}