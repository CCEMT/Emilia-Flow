﻿using System;
using Emilia.DataBuildPipeline.Editor;

namespace Emilia.Flow.Editor
{
    public class FlowBuildArgs : BuildArgs
    {
        public EditorFlowAsset flowAsset;
        public string outputPath;
        
        public bool isGenerateFile;
        public bool updateRunner = true;

        public FlowBuildArgs(EditorFlowAsset flowAsset, string outputPath, Action<BuildReport> onBuildComplete = null)
        {
            this.flowAsset = flowAsset;
            this.outputPath = outputPath;
            this.onBuildComplete = onBuildComplete;
            isGenerateFile = true;
        }
    }
}