using System;
using Emilia.Flow.Attributes;
using Emilia.Flow.Emilia;
using Emilia.Kit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Emilia.Statescript
{
    [FlowNodeMenu("状态/等待"), Serializable]
    public class WaitStatescriptStateAsset : StatescriptStateAsset<WaitStatescriptState>, IObjectDescription
    {
        [LabelText("等待时间")]
        public float time;

        public string description => $"等待时间(s):{this.time}";
    }

    [FlowNodeGenerator]
    public partial class WaitStatescriptState : StatescriptState<WaitStatescriptStateAsset>
    {
        private bool isFinish;
        private float timer;

        protected override void OnStart()
        {
            base.OnStart();
            isFinish = false;
            timer = 0;
        }

        [FlowOutputMethodPort("完成时", FlowPortCapacity.MultiConnect), FlowPortOrder(110)]
        protected void OnFinish()
        {
            FlowDebugUtility.Ping(this, "Finish");

            InvokeOutputPort(nameof(OnFinish));
        }

        protected override void OnTick()
        {
            base.OnTick();

            if (isFinish) return;

            this.timer += Time.deltaTime;
            if (this.timer >= this.asset.time)
            {
                OnFinish();
                isFinish = true;
            }
        }
    }
}