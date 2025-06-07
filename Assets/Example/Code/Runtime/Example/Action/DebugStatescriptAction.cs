using System;
using Emilia.Flow.Attributes;
using Emilia.Kit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Emilia.Statescript
{
    [FlowNodeMenu("行为/调试"), Serializable]
    public class DebugStatescriptActionAsset : StatescriptActionAsset<DebugStatescriptAction>, IObjectDescription
    {
        [LabelText("消息")]
        public string message;

        public string description => message;
    }

    public class DebugStatescriptAction : StatescriptAction<DebugStatescriptActionAsset>
    {
        protected override void OnExecute()
        {
            Debug.Log(this.asset.message);
        }
    }
}