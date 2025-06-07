using System;
using Emilia.Flow.Attributes;
using Emilia.Kit;
using UnityEngine;

namespace Emilia.Statescript
{
    [FlowNodeMenu("行为/设置玩家颜色"), Serializable]
    public class SetPlayerColorActionAsset : StatescriptActionAsset<SetPlayerColorAction>, IObjectDescription
    {
        public Color color;
        public string description => $"<color=#{ColorUtility.ToHtmlStringRGB(this.color)}>颜色</color>";
    }

    public class SetPlayerColorAction : StatescriptAction<SetPlayerColorActionAsset>
    {
        private Player player;

        protected override void OnInit()
        {
            base.OnInit();
            GameObject ownerGameObject = graph.owner as GameObject;
            player = ownerGameObject.GetComponent<Player>();
        }

        protected override void OnExecute()
        {
            player.SetColor(this.asset.color);
        }
    }
}