using System;
using Emilia.BehaviorTree.Attributes;
using Emilia.Flow.Attributes;
using Emilia.Node.Attributes;
using Emilia.Variables;
using Sirenix.OdinInspector;

namespace Emilia.Statescript
{
    [FlowNodeMenu("条件/变量比较"), Serializable]
    public class VariableComparisonStatescriptConditionAsset : StatescriptConditionAsset<VariableComparisonStatescriptCondition>
    {
        [LabelText("比较值"), VariableKeySelector]
        public string leftKey;

        [LabelText("比较操作符")]
        public VariableCompareOperator compareOperator;

        [HideLabel, HorizontalGroup(20)]
        public bool useDefine = true;

        [HorizontalGroup, VariableTypeFilter(nameof(leftKey)), ShowIf(nameof(useDefine))]
        public Variable rightDefineValue = new VariableObject();

        [LabelText("比较值"), VariableKeySelector, HorizontalGroup, HideIf(nameof(useDefine))]
        public string rightKey;
    }

    public class VariableComparisonStatescriptCondition : StatescriptCondition<VariableComparisonStatescriptConditionAsset>
    {
        protected override void OnInput(object arg)
        {
            base.OnInput(arg);
            Variable leftValue = graph.variablesManage.GetVariable(this.asset.leftKey);
            Variable rightValue = this.asset.useDefine ? this.asset.rightDefineValue : graph.variablesManage.GetVariable(this.asset.rightKey);

            bool result = false;
            if (leftValue != null && rightValue != null) result = VariableUtility.Compare(leftValue, rightValue, this.asset.compareOperator);

            if (result) OnTrue();
            else OnFalse();
        }
    }
}