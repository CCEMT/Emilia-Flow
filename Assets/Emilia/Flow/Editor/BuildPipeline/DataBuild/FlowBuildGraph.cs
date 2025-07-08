using System;
using Emilia.DataBuildPipeline.Editor;

namespace Emilia.Flow.Editor
{
    [BuildPipeline(typeof(FlowBuildArgs)), BuildSequence(4000)]
    public class FlowBuildGraph : IDataBuild
    {
        public void Build(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            FlowBuildContainer container = buildContainer as FlowBuildContainer;
            FlowBuildArgs flowBuildArgs = buildArgs as FlowBuildArgs;

            if (string.IsNullOrEmpty(flowBuildArgs.flowAsset.id)) flowBuildArgs.flowAsset.id = Guid.NewGuid().ToString();

            string id = flowBuildArgs.flowAsset.id;
            string description = flowBuildArgs.flowAsset.description;

            container.flowGraphAsset = new FlowGraphAsset(id, description, container.nodes, container.edges, container.variablesManage);
            onFinished.Invoke();
        }
    }
}