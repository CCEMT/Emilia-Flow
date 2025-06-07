using System;
using Emilia.Flow.Attributes;
using Emilia.Flow.Emilia;
using Emilia.Node.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Emilia.Statescript
{
    [FlowNodeMenu("状态/摇杆"), Serializable]
    public class JoyStatescriptStateAsset : StatescriptStateAsset<JoyStatescriptState>
    {
        [LabelText("摇杆值"), VariableKeySelector, VariableKeyTypeFilter(typeof(Vector2))]
        public string joyValueKey;
    }

    public class JoyStatescriptState : StatescriptState<JoyStatescriptStateAsset>
    {
        private StatescriptPulse nonePulse = new StatescriptPulse();
        private StatescriptPulse inputPulse = new StatescriptPulse();

        protected override void OnTick()
        {
            base.OnTick();

            float horizontalValue = Input.GetAxis("Horizontal");
            float verticalValue = Input.GetAxis("Vertical");
            Vector2 joyValue = new Vector2(horizontalValue, verticalValue);

            graph.variablesManage.SetValue(this.asset.joyValueKey, joyValue);

            if (joyValue == Vector2.zero)
            {
                nonePulse.Done();
                OnNone();
            }
            else
            {
                inputPulse.Done();
                OnInput();
            }

        }

        [FlowOutputMethodPort("无输入时子输出", FlowPortCapacity.MultiConnect), FlowPortOrder(110)]
        private void OnNone()
        {
            FlowDebugUtility.Ping(this, "None");

            this.nonePulse.Clear();
            InvokeOutputPort(nameof(OnNone), this.nonePulse);
        }

        [FlowOutputMethodPort("有输入时子输出", FlowPortCapacity.MultiConnect), FlowPortOrder(120)]
        private void OnInput()
        {
            FlowDebugUtility.Ping(this, "Input");

            this.inputPulse.Clear();
            InvokeOutputPort(nameof(OnInput), this.inputPulse);
        }

        protected override void OnDispose()
        {
            this.nonePulse.Done();
            this.inputPulse.Done();

            base.OnDispose();
        }
    }
}