using System;
using System.Collections.Generic;
using System.Linq;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Node.Editor;

namespace Emilia.Flow.Editor
{
    [BuildPipeline(typeof(FlowBuildArgs)), BuildSequence(3000)]
    public class FlowBuildEdge : IDataBuild
    {
        public void Build(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            FlowBuildContainer container = buildContainer as FlowBuildContainer;
            FlowBuildArgs flowBuildArgs = buildArgs as FlowBuildArgs;

            List<FlowEdgeAsset> edges = new();

            Dictionary<int, FlowEdgeAsset> edgeById = new();
            Dictionary<FlowEdgeAsset, float> priorityByEdge = new();

            int id = 0;

            int amount = flowBuildArgs.flowAsset.edges.Count;
            for (int i = 0; i < amount; i++)
            {
                EditorEdgeAsset edge = flowBuildArgs.flowAsset.edges[i];

                id++;

                EditorNodeAsset editorInputNode = flowBuildArgs.flowAsset.nodeMap.GetValueOrDefault(edge.inputNodeId);
                EditorNodeAsset editorOutputNode = flowBuildArgs.flowAsset.nodeMap.GetValueOrDefault(edge.outputNodeId);
                if (editorInputNode == null || editorOutputNode == null) continue;

                FlowNodeAsset inputNode = container.nodeMap.GetValueOrDefault(editorInputNode.id);
                FlowNodeAsset outputNode = container.nodeMap.GetValueOrDefault(editorOutputNode.id);
                if (inputNode == null || outputNode == null) continue;

                FlowPortAsset inputFlowPortAsset = inputNode.inputPorts.FirstOrDefault((x) => x.portName == edge.inputPortId);
                FlowPortAsset outputFlowPortAsset = outputNode.outputPorts.FirstOrDefault((x) => x.portName == edge.outputPortId);
                if (inputFlowPortAsset == null || outputFlowPortAsset == null) continue;

                float priority = editorInputNode.position.y + editorOutputNode.position.y;

                FlowEdgeAsset flowEdge = new(id, inputNode.id, outputNode.id, edge.inputPortId, edge.outputPortId);
                priorityByEdge.Add(flowEdge, priority);
                edgeById[id] = flowEdge;

                List<int> inputEdgeIds = inputFlowPortAsset.edgeIds as List<int>;
                inputEdgeIds.Add(flowEdge.id);
                inputEdgeIds.Sort((a, b) => {
                    float aPriority = priorityByEdge[edgeById[a]];
                    float bPriority = priorityByEdge[edgeById[b]];
                    return aPriority.CompareTo(bPriority);
                });

                List<int> outputEdgeIds = outputFlowPortAsset.edgeIds as List<int>;
                outputEdgeIds.Add(flowEdge.id);
                outputEdgeIds.Sort((a, b) => {
                    float aPriority = priorityByEdge[edgeById[a]];
                    float bPriority = priorityByEdge[edgeById[b]];
                    return aPriority.CompareTo(bPriority);
                });

                edges.Add(flowEdge);
            }

            edges.Sort((a, b) => {
                float aPriority = priorityByEdge[a];
                float bPriority = priorityByEdge[b];
                return aPriority.CompareTo(bPriority);
            });

            container.edges.AddRange(edges);

            onFinished.Invoke();
        }
    }
}