using System;
using Emilia.Flow.Attributes;
using Emilia.Flow.Emilia;
using Emilia.Kit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Emilia.Statescript
{
    [FlowNodeMenu("状态/按钮"), Serializable]
    public class ButtonStatescriptStateAsset : StatescriptStateAsset<ButtonStatescriptState>, IObjectDescription
    {
        [LabelText("按键")]
        public KeyCode keyCode;

        public string description => $"按键：{this.keyCode}";
    }

    public class ButtonStatescriptState : StatescriptState<ButtonStatescriptStateAsset>
    {
        private StatescriptPulse downPulse = new();
        private StatescriptPulse upPulse = new();

        protected override void OnStart()
        {
            base.OnStart();
            downPulse.Clear();
            upPulse.Clear();
        }

        protected override void OnTick()
        {
            base.OnTick();

            if (Input.GetKeyDown(this.asset.keyCode))
            {
                this.upPulse.Done();
                OnDown();
            }

            if (Input.GetKeyUp(this.asset.keyCode))
            {
                this.downPulse.Done();
                OnUp();
            }
        }

        [FlowOutputMethodPort("按下时子输出", FlowPortCapacity.MultiConnect), FlowPortOrder(110)]
        protected void OnDown()
        {
            FlowDebugUtility.Ping(this, "Down");

            downPulse.Clear();
            InvokeOutputPort(nameof(OnDown), this.downPulse);
        }

        [FlowOutputMethodPort("抬起时子输出", FlowPortCapacity.MultiConnect), FlowPortOrder(120)]
        protected void OnUp()
        {
            FlowDebugUtility.Ping(this, "Up");

            upPulse.Clear();
            InvokeOutputPort(nameof(OnUp), this.upPulse);
        }

        protected override void OnDispose()
        {
            this.upPulse.Done();
            this.downPulse.Done();

            base.OnDispose();
        }
    }
}