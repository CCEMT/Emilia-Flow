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
        public void Build(IBuildContainer buildContainer, Action onFinished)
        {
            FlowBuildContainer container = buildContainer as FlowBuildContainer;

            List<FlowEdgeAsset> edges = new List<FlowEdgeAsset>();

            Dictionary<int, FlowEdgeAsset> edgeById = new Dictionary<int, FlowEdgeAsset>();
            Dictionary<FlowEdgeAsset, float> priorityByEdge = new Dictionary<FlowEdgeAsset, float>();

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
                
                FlowPortAsset inputFlowPortAsset = inputNode.inputPorts.FirstOrDefault((x) => x.portName == edge.inputPortId);
                FlowPortAsset outputFlowPortAsset = outputNode.outputPorts.FirstOrDefault((x) => x.portName == edge.outputPortId);
                if (inputFlowPortAsset == null || outputFlowPortAsset == null) continue;
                
                float priority = editorInputNode.position.y + editorOutputNode.position.y;

                FlowEdgeAsset flowEdge = new FlowEdgeAsset(id, inputNode.id, outputNode.id, edge.inputPortId, edge.outputPortId);
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