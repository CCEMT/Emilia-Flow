using System;
using Emilia.Kit;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using Emilia.Variables.Editor;

namespace Emilia.Statescript.Editor
{
    [ObjectDescription(typeof(SetPlayerMoveDirectionActionAsset)), Serializable]
    public class SetPlayerMoveDirectionActionAssetDescription : IObjectDescriptionGetter
    {
        public string GetDescription(object obj, object owner, object userData)
        {
            SetPlayerMoveDirectionActionAsset asset = obj as SetPlayerMoveDirectionActionAsset;
            EditorGraphView editorGraphView = owner as EditorGraphView;

            EditorUniversalGraphAsset universalGraphAsset = editorGraphView.graphAsset as EditorUniversalGraphAsset;
            if (universalGraphAsset == null) return string.Empty;
            if (universalGraphAsset.editorParametersManage == null) return string.Empty;

            EditorParameter editorParameter = universalGraphAsset.editorParametersManage.GetParameter(asset.key);
            if (editorParameter == null) return string.Empty;
            return editorParameter.description;
        }
    }
}