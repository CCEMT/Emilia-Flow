using System;
using System.Collections.Generic;
using Emilia.Node.Universal.Editor;
using Sirenix.Serialization;
using UnityEngine;

namespace Emilia.Flow.Editor
{
    [NodeToRuntime(typeof(FlowNodeAsset), typeof(EditorFlowNodeAsset))]
    public abstract class EditorFlowAsset : EditorUniversalGraphAsset
    {
        [NonSerialized, OdinSerialize]
        public FlowGraphAsset cache;

        [NonSerialized, OdinSerialize, HideInInspector]
        public Dictionary<int, string> cacheEditorByRuntimeIdMap = new();

        [NonSerialized, OdinSerialize, HideInInspector]
        public Dictionary<string, int> cacheRuntimeByEditorIdMap = new();

        public abstract string outputPath { get; }

        public abstract Type[] subNodeTypes { get; }
        public virtual FlowCapacity capacity => FlowCapacity.None;
    }
}