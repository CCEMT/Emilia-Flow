using System;
using Emilia.BehaviorTree.Attributes;
using Emilia.Flow.Attributes;
using Emilia.Node.Attributes;
using Emilia.Variables;
using Sirenix.OdinInspector;

namespace Emilia.Statescript
{
    [FlowNodeMenu("行为/变量运算"), Serializable]
    public class SetVariableStatescriptActionAsset : StatescriptActionAsset<SetVariableStatescriptAction>
    {
        [LabelText("参数"), VariableKeySelector]
        public string leftKey;

        [LabelText("运算符")]
        public VariableCalculateOperator calculateOperator;

        [HideLabel, HorizontalGroup(20)]
        public bool useDefine = true;

        [HorizontalGroup, VariableTypeFilter(nameof(leftKey)), ShowIf(nameof(useDefine))]
        public Variable rightDefineValue = new VariableObject();

        [LabelText("参数"), VariableKeySelector, HorizontalGroup, HideIf(nameof(useDefine))]
        public string rightKey;
    }

    public class SetVariableStatescriptAction : StatescriptAction<SetVariableStatescriptActionAsset>
    {
        protected override void OnExecute()
        {
            Variable leftValue = graph.variablesManage.GetVariable(this.asset.leftKey);
            Variable rightValue = this.asset.useDefine ? this.asset.rightDefineValue : graph.variablesManage.GetVariable(this.asset.rightKey);
            if (leftValue != null && rightValue != null)
            {
                Variable result = VariableUtility.Calculate(leftValue, rightValue, this.asset.calculateOperator);
                graph.variablesManage.Set(this.asset.leftKey, result);
            }
        }
    }
}