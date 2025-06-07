using System;
using Emilia.Flow;
using Emilia.Flow.Attributes;
using Emilia.Flow.Emilia;
using Emilia.Node.Attributes;

namespace Emilia.Statescript
{
    [Serializable, NodeColor(0, 1, 0)]
    public abstract class StatescriptConditionAsset<T> : StatescriptNodeAsset<T> where T : FlowNode, new() { }

    [FlowNodeGenerator]
    public abstract partial class StatescriptCondition<T> : StatescriptNode<T> where T : FlowNodeAsset
    {
        protected object pass;

        [FlowInputMethodPort("输入", FlowPortCapacity.MultiConnect)]
        protected virtual void OnInput(object arg)
        {
            pass = arg;
        }

        [FlowOutputMethodPort("是", FlowPortCapacity.MultiConnect)]
        protected virtual void OnTrue()
        {
            FlowDebugUtility.Ping(this, "True");

            InvokeOutputPort(nameof(OnTrue), this.pass);
        }

        [FlowOutputMethodPort("否", FlowPortCapacity.MultiConnect)]
        protected virtual void OnFalse()
        {
            FlowDebugUtility.Ping(this, "False");

            InvokeOutputPort(nameof(OnFalse), pass);
        }
    }
}