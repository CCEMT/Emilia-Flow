using System;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Node.Editor;

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

            container.editorFlowAsset.Save();

            onFinished.Invoke();
        }
    }
}