using System;
using Emilia.Flow.Attributes;
using Emilia.Node.Attributes;
using UnityEngine;

namespace Emilia.Statescript
{
    [FlowNodeMenu("行为/设置玩家移动方向"), Serializable]
    public class SetPlayerMoveDirectionActionAsset : StatescriptActionAsset<SetPlayerMoveDirectionAction>
    {
        [VariableKeySelector, VariableKeyTypeFilter(typeof(Vector2))]
        public string key;
    }

    public class SetPlayerMoveDirectionAction : StatescriptAction<SetPlayerMoveDirectionActionAsset>
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
            player.moveDirection = graph.variablesManage.GetValue<Vector2>(this.asset.key);
        }
    }
}