using System;
using Emilia.Flow.Attributes;
using Emilia.Kit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Emilia.Statescript
{
    [FlowNodeMenu("行为/玩家攻击"), Serializable]
    public class PlayerAttackActionAsset : StatescriptActionAsset<PlayerAttackAction>, IObjectDescription
    {
        [LabelText("是否重击")]
        public bool isHeavy;

        public string description => isHeavy ? "重击" : "普通攻击";
    }

    public class PlayerAttackAction : StatescriptAction<PlayerAttackActionAsset>
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
            player.Attack(this.asset.isHeavy);
        }
    }
}