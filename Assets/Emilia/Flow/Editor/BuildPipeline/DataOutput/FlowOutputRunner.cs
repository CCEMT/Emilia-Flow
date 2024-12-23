using System;
using System.Collections.Generic;
using Emilia.DataBuildPipeline.Editor;
using UnityEditor;

namespace Emilia.Flow.Editor
{
    [BuildPipeline(FlowBuildPipeline.PipelineName), BuildSequence(3000)]
    public class FlowOutputRunner : IDataOutput
    {
        public void Output(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            FlowBuildContainer container = buildContainer as FlowBuildContainer;
            FlowBuildArgs args = buildArgs as FlowBuildArgs;

            if (args.updateRunner == false || EditorApplication.isPlaying == false)
            {
                onFinished.Invoke();
                return;
            }

            List<EditorFlowRunner> runners = EditorFlowRunner.runnerByAssetId.GetValueOrDefault(container.editorFlowAsset.id);
            if (runners == null)
            {
                onFinished.Invoke();
                return;
            }

            int runnerCount = runners.Count;
            for (int i = 0; i < runnerCount; i++)
            {
                EditorFlowRunner runner = runners[i];
                if (runner == null) continue;
                if (runner.isActive == false) continue;
                runner.Reload(container.flowGraphAsset);
            }

            onFinished.Invoke();
        }
    }
}