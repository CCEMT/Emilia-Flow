using System;
using Emilia.Kit;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using Emilia.Variables.Editor;

namespace Emilia.Statescript.Editor
{
    [ObjectDescription(typeof(JoyStatescriptStateAsset)), Serializable]
    public class JoyStatescriptStateAssetDescription : IObjectDescriptionGetter
    {
        public string GetDescription(object obj, object owner, object userData)
        {
            JoyStatescriptStateAsset asset = obj as JoyStatescriptStateAsset;
            EditorGraphView editorGraphView = owner as EditorGraphView;

            EditorUniversalGraphAsset universalGraphAsset = editorGraphView.graphAsset as EditorUniversalGraphAsset;
            if (universalGraphAsset == null) return string.Empty;
            if (universalGraphAsset.editorParametersManage == null) return string.Empty;

            EditorParameter editorParameter = universalGraphAsset.editorParametersManage.GetParameter(asset.joyValueKey);
            if (editorParameter == null) return string.Empty;
            return editorParameter.description;
        }
    }
}