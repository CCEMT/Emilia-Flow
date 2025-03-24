using Emilia.DataBuildPipeline.Editor;

namespace Emilia.Flow.Editor
{
    [BuildPipeline(PipelineName)]
    public class FlowBuildPipeline : UniversalBuildPipeline
    {
        public const string PipelineName = "Flow";

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