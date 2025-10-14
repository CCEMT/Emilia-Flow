using System.Collections.Generic;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Variables;

namespace Emilia.Flow.Editor
{
    [BuildPipeline(typeof(FlowBuildArgs))]
    public class FlowBuildContainer : BuildContainer
    {
        public Dictionary<int, string> editorByRuntimeMap { get; set; } = new();
        public Dictionary<string, int> runtimeByEditorMap { get; set; } = new();

        public List<FlowNodeAsset> nodes { get; set; } = new();
        public List<FlowEdgeAsset> edges { get; set; } = new();
        public VariablesManager variablesManage { get; set; }

        public Dictionary<string, FlowNodeAsset> nodeMap { get; set; } = new();

        public FlowGraphAsset flowGraphAsset { get; set; }
    }
}