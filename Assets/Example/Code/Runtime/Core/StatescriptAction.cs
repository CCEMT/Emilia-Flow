using System;
using Emilia.Flow;
using Emilia.Flow.Attributes;
using Emilia.Flow.Emilia;
using Emilia.Node.Attributes;

namespace Emilia.Statescript
{
    [Serializable, NodeColor(1, 0, 0)]
    public abstract class StatescriptActionAsset<T> : StatescriptNodeAsset<T> where T : FlowNode, new() { }

    [FlowNodeGenerator]
    public abstract partial class StatescriptAction<T> : StatescriptNode<T> where T : FlowNodeAsset
    {
        protected object pass;

        [FlowInputMethodPort("输入", FlowPortCapacity.MultiConnect)]
        protected virtual void OnInput(object arg)
        {
            FlowDebugUtility.Ping(this, "Execute");

            pass = arg;
            OnExecute();
            OnOutput();
        }

        [FlowOutputMethodPort("输出", FlowPortCapacity.MultiConnect)]
        protected virtual void OnOutput()
        {
            InvokeOutputPort(nameof(OnOutput), pass);
        }

        protected abstract void OnExecute();
    }
}