using System;
using Emilia.DataBuildPipeline.Editor;

namespace Emilia.Flow.Editor
{
    public static class EditorFlowUtility
    {
        public static void DataBuild(EditorFlowAsset flowAsset, Action<BuildReport> onBuildComplete = null)
        {
            if (flowAsset == null) return;

            string path = flowAsset.outputPath;

            FlowBuildArgs flowBuildArgs = new FlowBuildArgs(flowAsset, path);
            flowBuildArgs.onBuildComplete = onBuildComplete;

            DataBuildUtility.Build(flowBuildArgs);
        }
    }
}