﻿using System;
using System.Collections.Generic;
using System.Linq;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Node.Editor;

namespace Emilia.Flow.Editor
{
    [BuildPipeline(FlowBuildPipeline.PipelineName), BuildSequence(3000)]
    public class FlowBuildEdge : IDataBuild
    {
        public void Build(IBuildContainer buildContainer, Action onFinished)
        {
            FlowBuildContainer container = buildContainer as FlowBuildContainer;

            List<FlowEdgeAsset> edges = new List<FlowEdgeAsset>();

            int id = 0;

            int amount = container.editorFlowAsset.edges.Count;
            for (int i = 0; i < amount; i++)
            {
                EditorEdgeAsset edge = container.editorFlowAsset.edges[i];

                id++;

                EditorNodeAsset editorInputNode = container.editorFlowAsset.nodeMap.GetValueOrDefault(edge.inputNodeId);
                EditorNodeAsset editorOutputNode = container.editorFlowAsset.nodeMap.GetValueOrDefault(edge.outputNodeId);
                if (editorInputNode == null || editorOutputNode == null) continue;

                FlowNodeAsset inputNode = container.nodeMap.GetValueOrDefault(editorInputNode.id);
                FlowNodeAsset outputNode = container.nodeMap.GetValueOrDefault(editorOutputNode.id);
                if (inputNode == null || outputNode == null) continue;

                FlowEdgeAsset flowEdge = new FlowEdgeAsset(id, inputNode.id, outputNode.id, edge.inputPortId, edge.outputPortId);

                FlowPortAsset inputFlowPortAsset = inputNode.inputPorts.FirstOrDefault((x) => x.portName == edge.inputPortId);
                List<int> inputEdgeIds = inputFlowPortAsset.edgeIds as List<int>;
                inputEdgeIds.Add(flowEdge.id);

                FlowPortAsset outputFlowPortAsset = outputNode.outputPorts.FirstOrDefault((x) => x.portName == edge.outputPortId);
                List<int> outputEdgeIds = outputFlowPortAsset.edgeIds as List<int>;
                outputEdgeIds.Add(flowEdge.id);

                edges.Add(flowEdge);
            }

            container.edges.AddRange(edges);

            onFinished.Invoke();
        }
    }
}