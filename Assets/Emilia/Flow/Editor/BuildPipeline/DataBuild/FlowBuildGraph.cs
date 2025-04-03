using System;
using Emilia.DataBuildPipeline.Editor;

namespace Emilia.Flow.Editor
{
    [BuildPipeline(typeof(FlowBuildArgs)), BuildSequence(4000)]
    public class FlowBuildGraph : IDataBuild
    {
        public void Build(IBuildContainer buildContainer, Action onFinished)
        {
            FlowBuildContainer container = buildContainer as FlowBuildContainer;

            string id = container.editorFlowAsset.id;
            string description = container.editorFlowAsset.description;

            container.flowGraphAsset = new FlowGraphAsset(id, description, container.nodes, container.edges, container.variablesManage);
            onFinished.Invoke();
        }
    }
}