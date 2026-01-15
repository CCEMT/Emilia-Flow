using System;
using Emilia.Flow.Editor;
using UnityEngine;

namespace Emilia.Statescript.Editor
{
    [CreateAssetMenu(menuName = "Emilia/Statescript/EditorStatescriptAsset", fileName = "EditorStatescriptAsset")]
    public class EditorStatescriptAsset : EditorFlowAsset
    {
        public override string outputPath => "Assets/Example/Resource/Config";

        public override Type[] subNodeTypes => new[] {typeof(IStatescriptNodeAsset)};
    }
}