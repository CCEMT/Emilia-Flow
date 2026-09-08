using System;
using System.Collections.Generic;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Node.Editor;

namespace Emilia.Flow.Editor
{
    public interface IFlowBuildDynamicOutputPortResolver
    {
        bool TryResolveDynamicOutputPorts(
            EditorNodeAsset editorNodeAsset,
            FlowNodeAsset flowNodeAsset,
            out IReadOnlyList<FlowDynamicOutputPortDescriptor> ports);
    }

    public class FlowBuildArgs : BuildArgs
    {
        public EditorFlowAsset flowAsset;
        public string outputPath;

        public bool isGenerateFile;
        public bool updateRunner = true;
        public Action generateFileCallback;
        public IFlowBuildDynamicOutputPortResolver dynamicOutputPortResolver;

        public FlowBuildArgs(EditorFlowAsset flowAsset, string outputPath, Action<BuildReport> onBuildComplete = null)
        {
            this.flowAsset = flowAsset;
            this.outputPath = outputPath;
            this.onBuildComplete = onBuildComplete;
            this.isGenerateFile = true;
        }
    }
}
