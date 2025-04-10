using System;
using System.Collections.Generic;
using Emilia.Node.Universal.Editor;
using Sirenix.Serialization;

namespace Emilia.Flow.Editor
{
    [NodeToRuntime(typeof(FlowNodeAsset), typeof(EditorFlowNodeAsset))]
    public abstract class EditorFlowAsset : EditorUniversalGraphAsset
    {
        [NonSerialized, OdinSerialize]
        public FlowGraphAsset cache;

        [NonSerialized, OdinSerialize]
        public Dictionary<int, string> cacheEditorByRuntimeIdMap = new Dictionary<int, string>();
        
        [NonSerialized, OdinSerialize]
        public Dictionary<string, int> cacheRuntimeByEditorIdMap = new Dictionary<string, int>();
        
        public abstract string outputPath { get; }

        public abstract Type[] subNodeTypes { get; }
        public virtual FlowCapacity capacity => FlowCapacity.None;
    }
}