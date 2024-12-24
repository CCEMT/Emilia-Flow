using System;
using Emilia.DataBuildPipeline.Editor;

namespace Emilia.Flow.Editor
{
    [BuildPipeline(FlowBuildPipeline.PipelineName), BuildSequence(1000)]
    public class FlowOutputEditor : IDataOutput
    {
        public void Output(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            FlowBuildContainer container = buildContainer as FlowBuildContainer;

            container.editorFlowAsset.cache = container.flowGraphAsset;
            container.editorFlowAsset.cacheBindMap = container.bindMap;

            onFinished.Invoke();
        }
    }
}