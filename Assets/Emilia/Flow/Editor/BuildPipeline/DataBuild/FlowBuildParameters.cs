using System;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Variables;

namespace Emilia.Flow.Editor
{
    [BuildPipeline(typeof(FlowBuildArgs)), BuildSequence(1000)]
    public class FlowBuildParameters : IDataBuild
    {
        public void Build(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            FlowBuildContainer container = buildContainer as FlowBuildContainer;
            FlowBuildArgs flowBuildArgs = buildArgs as FlowBuildArgs;

            if (flowBuildArgs.flowAsset.editorParametersManage == null) container.variablesManage = new VariablesManager();
            else
            {
                VariablesManager rootVariablesManage = flowBuildArgs.flowAsset.editorParametersManage.ToParametersManage();
                container.variablesManage = rootVariablesManage;
            }

            onFinished.Invoke();
        }
    }
}