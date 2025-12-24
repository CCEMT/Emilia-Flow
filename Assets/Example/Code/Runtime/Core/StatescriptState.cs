using System;
using Emilia.Flow;
using Emilia.Flow.Attributes;
using Emilia.Flow.Emilia;
using Emilia.Node.Attributes;

namespace Emilia.Statescript
{
    public interface IStatescriptState { }

    [Serializable, NodeColor(1, 1, 0)]
    public abstract class StatescriptStateAsset<T> : StatescriptNodeAsset<T> where T : FlowNode, new() { }

    [FlowNodeGenerator]
    public abstract partial class StatescriptState<T> : StatescriptNode<T>, IStatescriptState where T : FlowNodeAsset
    {
        protected object pass;
        protected StatescriptPulse statePulse = new();

        [FlowInputMethodPort("开启", FlowPortCapacity.MultiConnect), FlowPortOrder(100)]
        protected virtual void Start(object arg)
        {
            FlowDebugUtility.SetState(this, true);

            StatescriptPulse pulse = arg as StatescriptPulse;
            if (pulse != null) pulse.onDone += Abort;

            pass = arg;

            statePulse.Clear();

            OnStart();
            OnSub();

            graph.onTick += OnTick;
        }

        [FlowInputMethodPort("中断", FlowPortCapacity.MultiConnect), FlowPortOrder(200)]
        protected virtual void Abort()
        {
            OnEnd();
        }

        [FlowOutputMethodPort("开启时", FlowPortCapacity.MultiConnect), FlowPortOrder(100)]
        protected virtual void OnStart()
        {
            InvokeOutputPort(nameof(OnStart), pass);
        }

        [FlowOutputMethodPort("轮询", FlowPortCapacity.MultiConnect), FlowPortOrder(200)]
        protected virtual void OnTick()
        {
            InvokeOutputPort(nameof(OnTick), pass);
        }

        [FlowOutputMethodPort("子输出", FlowPortCapacity.MultiConnect), FlowPortOrder(300)]
        protected virtual void OnSub()
        {
            InvokeOutputPort(nameof(OnSub), statePulse);
        }

        [FlowOutputMethodPort("关闭时", FlowPortCapacity.MultiConnect), FlowPortOrder(400)]
        protected virtual void OnEnd()
        {
            FlowDebugUtility.SetState(this, false);

            InvokeOutputPort(nameof(OnEnd), pass);

            graph.onTick -= OnTick;
            graph.onTick -= OnSub;

            statePulse.Done();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            graph.onTick -= OnTick;
        }
    }
}