using Emilia.DataBuildPipeline.Editor;

namespace Emilia.Flow.Editor
{
    [BuildPipeline(typeof(FlowBuildArgs))]
    public class FlowBuildPipeline : UniversalBuildPipeline
    {
        private FlowBuildArgs flowBuildArgs;

        protected override void RunInitialize()
        {
            base.RunInitialize();
            this.flowBuildArgs = this.buildArgs as FlowBuildArgs;
        }

        protected override IBuildContainer CreateContainer()
        {
            FlowBuildContainer flowBuildContainer = new FlowBuildContainer();
            flowBuildContainer.editorFlowAsset = this.flowBuildArgs.flowAsset;
            return flowBuildContainer;
        }
    }
}