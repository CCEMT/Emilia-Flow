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

            container.editorFlowAsset.cache = container.flowGraphAsset;
            container.editorFlowAsset.cacheEditorByRuntimeIdMap = container.editorByRuntimeMap;
            container.editorFlowAsset.cacheRuntimeByEditorIdMap = container.runtimeByEditorMap;

            container.editorFlowAsset.SaveAll();

            onFinished.Invoke();
        }
    }
}