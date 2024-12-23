using System.Collections.Generic;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Variables;

namespace Emilia.Flow.Editor
{
    public class FlowBuildContainer : BuildContainer
    {
        public EditorFlowAsset editorFlowAsset { get; set; }

        public Dictionary<int, string> bindMap { get; set; } = new Dictionary<int, string>();
        public List<FlowNodeAsset> nodes { get; set; } = new List<FlowNodeAsset>();
        public List<FlowEdgeAsset> edges { get; set; } = new List<FlowEdgeAsset>();
        public VariablesManage variablesManage { get; set; }

        public Dictionary<string, FlowNodeAsset> nodeMap { get; set; } = new Dictionary<string, FlowNodeAsset>();

        public FlowGraphAsset flowGraphAsset { get; set; }
    }
}