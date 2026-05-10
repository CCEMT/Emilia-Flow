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
            HashSet<string> buildEdgeKeys = new();

            int id = 0;

            int amount = flowBuildArgs.flowAsset.edges.Count;
            for (int i = 0; i < amount; i++)
            {
                EditorEdgeAsset edge = flowBuildArgs.flowAsset.edges[i];
                List<EditorLogicalConnection> logicalConnections = ResolveBuildEdgeConnections(flowBuildArgs.flowAsset, edge);

                int connectionCount = logicalConnections.Count;
                for (int j = 0; j < connectionCount; j++)
                {
                    EditorLogicalConnection logicalConnection = logicalConnections[j];
                    if (logicalConnection.outputNode == null || logicalConnection.inputNode == null) continue;

                    FlowNodeAsset inputNode = container.nodeMap.GetValueOrDefault(logicalConnection.inputNode.id);
                    FlowNodeAsset outputNode = container.nodeMap.GetValueOrDefault(logicalConnection.outputNode.id);
                    if (inputNode == null || outputNode == null) continue;

                    FlowPortAsset inputFlowPortAsset = inputNode.inputPorts.FirstOrDefault((x) => x.portName == logicalConnection.inputPortId);
                    FlowPortAsset outputFlowPortAsset = outputNode.outputPorts.FirstOrDefault((x) => x.portName == logicalConnection.outputPortId);
                    if (inputFlowPortAsset == null || outputFlowPortAsset == null) continue;

                    if (inputFlowPortAsset.edgeIds is not List<int> inputEdgeIds) continue;
                    if (outputFlowPortAsset.edgeIds is not List<int> outputEdgeIds) continue;

                    string buildEdgeKey = CreateBuildEdgeKey(logicalConnection);
                    if (buildEdgeKeys.Add(buildEdgeKey) == false) continue;

                    id++;

                    float priority = logicalConnection.inputNode.position.y + logicalConnection.outputNode.position.y;

                    FlowEdgeAsset flowEdge = new(id, inputNode.id, outputNode.id, logicalConnection.inputPortId, logicalConnection.outputPortId);
                    priorityByEdge.Add(flowEdge, priority);
                    edgeById[id] = flowEdge;

                    inputEdgeIds.Add(flowEdge.id);
                    inputEdgeIds.Sort((a, b) => {
                        float aPriority = priorityByEdge[edgeById[a]];
                        float bPriority = priorityByEdge[edgeById[b]];
                        return aPriority.CompareTo(bPriority);
                    });

                    outputEdgeIds.Add(flowEdge.id);
                    outputEdgeIds.Sort((a, b) => {
                        float aPriority = priorityByEdge[edgeById[a]];
                        float bPriority = priorityByEdge[edgeById[b]];
                        return aPriority.CompareTo(bPriority);
                    });

                    edges.Add(flowEdge);
                }
            }

            edges.Sort((a, b) => {
                float aPriority = priorityByEdge[a];
                float bPriority = priorityByEdge[b];
                return aPriority.CompareTo(bPriority);
            });

            container.edges.AddRange(edges);

            onFinished.Invoke();
        }

        private static List<EditorLogicalConnection> ResolveBuildEdgeConnections(EditorGraphAsset graphAsset, EditorEdgeAsset edge)
        {
            List<EditorLogicalConnection> result = new();

            EditorNodeAsset outputNode = graphAsset.nodeMap.GetValueOrDefault(edge.outputNodeId);
            if (outputNode == null) return result;

            List<EditorLogicalConnection> logicalConnections = outputNode.GetLogicalOutputNodes(new HashSet<string>());
            int count = logicalConnections.Count;
            for (int i = 0; i < count; i++)
            {
                EditorLogicalConnection logicalConnection = logicalConnections[i];
                if (logicalConnection.outputNode == null) continue;
                if (logicalConnection.inputNode == null) continue;
                if (logicalConnection.outputNode.id != edge.outputNodeId) continue;
                if (logicalConnection.outputPortId != edge.outputPortId) continue;
                if (string.IsNullOrEmpty(logicalConnection.inputPortId)) continue;

                result.Add(logicalConnection);
            }

            return result;
        }

        private static string CreateBuildEdgeKey(EditorLogicalConnection logicalConnection)
        {
            return $"{logicalConnection.outputNode.id}|{logicalConnection.inputNode.id}|{logicalConnection.outputPortId}|{logicalConnection.inputPortId}";
        }
    }
}
