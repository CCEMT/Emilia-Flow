using System;
using Emilia.Flow;
using Emilia.Flow.Attributes;
using Emilia.Flow.Emilia;
using Emilia.Node.Attributes;

namespace Emilia.Statescript
{
    [Serializable, NodeColor(0, 0.5f, 1f)]
    public abstract class StatescriptEntryAsset<T> : StatescriptNodeAsset<T> where T : FlowNode, new() { }

    [FlowNodeGenerator]
    public abstract partial class StatescriptEntry<T> : StatescriptNode<T> where T : FlowNodeAsset
    {
        [FlowOutputMethodPort("输出", FlowPortCapacity.MultiConnect)]
        protected virtual void OnOutput()
        {
            FlowDebugUtility.Ping(this, "Out");

            InvokeOutputPort(nameof(OnOutput));
        }
    }
}