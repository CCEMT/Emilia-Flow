using System;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Variables;

namespace Emilia.Flow.Editor
{
    [BuildPipeline(FlowBuildPipeline.PipelineName), BuildSequence(1000)]
    public class FlowBuildParameters : IDataBuild
    {
        public void Build(IBuildContainer buildContainer, Action onFinished)
        {
            FlowBuildContainer container = buildContainer as FlowBuildContainer;

            if (container.editorFlowAsset.editorParametersManage == null) container.variablesManage = new VariablesManage();
            else
            {
                VariablesManage rootVariablesManage = container.editorFlowAsset.editorParametersManage.ToParametersManage();
                container.variablesManage = rootVariablesManage;
            }

            onFinished.Invoke();
        }
    }
}