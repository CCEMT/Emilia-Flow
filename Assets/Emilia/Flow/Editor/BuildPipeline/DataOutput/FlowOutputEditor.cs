using System;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Kit.Editor;

namespace Emilia.Flow.Editor
{
    [BuildPipeline(typeof(FlowBuildArgs)), BuildSequence(1000)]
    public class FlowOutputEditor : IDataOutput
    {
        public void Output(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            FlowBuildContainer container = buildContainer as FlowBuildContainer;
            FlowBuildArgs flowBuildArgs = buildArgs as FlowBuildArgs;

            flowBuildArgs.flowAsset.cache = container.flowGraphAsset;
            flowBuildArgs.flowAsset.cacheEditorByRuntimeIdMap = container.editorByRuntimeMap;
            flowBuildArgs.flowAsset.cacheRuntimeByEditorIdMap = container.runtimeByEditorMap;

            flowBuildArgs.flowAsset.SaveAll();

            onFinished.Invoke();
        }
    }
}