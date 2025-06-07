using System;
using Emilia.Kit;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using Emilia.Variables;
using Emilia.Variables.Editor;

namespace Emilia.Statescript.Editor
{
    [ObjectDescription(typeof(SetVariableStatescriptActionAsset)), Serializable]
    public class SetVariableStatescriptActionAssetDescription : IObjectDescriptionGetter
    {
        public string GetDescription(object obj, object owner, object userData)
        {
            SetVariableStatescriptActionAsset asset = obj as SetVariableStatescriptActionAsset;
            EditorGraphView editorGraphView = owner as EditorGraphView;

            EditorUniversalGraphAsset universalGraphAsset = editorGraphView.graphAsset as EditorUniversalGraphAsset;
            if (universalGraphAsset == null) return string.Empty;
            if (universalGraphAsset.editorParametersManage == null) return string.Empty;

            EditorParameter leftEditorParameter = universalGraphAsset.editorParametersManage.GetParameter(asset.leftKey);
            if (leftEditorParameter == null) return string.Empty;

            EditorParameter rightEditorParameter = universalGraphAsset.editorParametersManage.GetParameter(asset.rightKey);

            string leftDescription = leftEditorParameter.description;
            string rightDescription = asset.useDefine ? asset.rightDefineValue.ToString() : rightEditorParameter?.description;
            return leftDescription + VariableUtility.ToDisplayString(asset.calculateOperator) + rightDescription;
        }
    }
}