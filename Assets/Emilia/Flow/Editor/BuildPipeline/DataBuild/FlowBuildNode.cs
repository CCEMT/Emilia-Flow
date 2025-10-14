using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Flow.Attributes;
using Emilia.Kit.Editor;
using Emilia.Node.Editor;
using Sirenix.Serialization;

namespace Emilia.Flow.Editor
{
    [BuildPipeline(typeof(FlowBuildArgs)), BuildSequence(2000)]
    public class FlowBuildNode : IDataBuild
    {
        public void Build(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            FlowBuildContainer container = buildContainer as FlowBuildContainer;
            FlowBuildArgs flowBuildArgs = buildArgs as FlowBuildArgs;

            int id = 0;

            int amount = flowBuildArgs.flowAsset.nodes.Count;
            for (int i = 0; i < amount; i++)
            {
                EditorNodeAsset editorNodeAsset = flowBuildArgs.flowAsset.nodes[i];

                FlowNodeAsset flowNodeAsset = editorNodeAsset.userData as FlowNodeAsset;
                if (flowNodeAsset == default) continue;

                FlowNodeAsset copy = SerializationUtility.CreateCopy(flowNodeAsset) as FlowNodeAsset;

                id++;
                ReflectUtility.SetValue(typeof(FlowNodeAsset), copy, nameof(copy.id), id);

                IUniversalFlowNodeAsset universalFlowNodeAsset = copy as IUniversalFlowNodeAsset;
                if (universalFlowNodeAsset == default) continue;

                Type nodeType = universalFlowNodeAsset.nodeType;

                List<FlowPortAsset> inputPorts = copy.inputPorts as List<FlowPortAsset>;
                List<FlowPortAsset> outputPorts = copy.outputPorts as List<FlowPortAsset>;

                inputPorts.Clear();
                outputPorts.Clear();

                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

                FieldInfo[] fileInfos = copy.GetType().GetFields(bindingFlags);
                int fieldAmount = fileInfos.Length;
                for (int j = 0; j < fieldAmount; j++)
                {
                    FieldInfo fieldInfo = fileInfos[j];
                    FlowInputValuePort flowInputAttribute = fieldInfo.GetCustomAttribute<FlowInputValuePort>(true);
                    if (flowInputAttribute == null) continue;

                    FlowPortAsset portAsset = new(fieldInfo.Name, new List<int>());
                    inputPorts.Add(portAsset);
                }

                MethodInfo[] methods = nodeType.GetMethods(bindingFlags);
                int methodAmount = methods.Length;
                for (int j = 0; j < methodAmount; j++)
                {
                    MethodInfo methodInfo = methods[j];

                    FlowOutputValuePort flowOutputAttribute = methodInfo.GetCustomAttribute<FlowOutputValuePort>(true);
                    if (flowOutputAttribute != null)
                    {
                        FlowPortAsset valuePortAsset = new(methodInfo.Name, new List<int>());
                        outputPorts.Add(valuePortAsset);
                        continue;
                    }

                    bool? inputOrOutput = null;
                    FlowInputMethodPort flowInputMethodAttribute = methodInfo.GetCustomAttribute<FlowInputMethodPort>(true);
                    if (flowInputMethodAttribute != null) inputOrOutput = true;

                    FlowOutputMethodPort flowOutputMethodAttribute = methodInfo.GetCustomAttribute<FlowOutputMethodPort>(true);
                    if (flowOutputMethodAttribute != null) inputOrOutput = false;

                    if (inputOrOutput == null) continue;

                    FlowPortAsset portAsset = new(methodInfo.Name, new List<int>());
                    if (inputOrOutput.Value) inputPorts.Add(portAsset);
                    else outputPorts.Add(portAsset);
                }

                FilterPort(universalFlowNodeAsset, inputPorts);
                FilterPort(universalFlowNodeAsset, outputPorts);

                container.nodes.Add(copy);
                container.nodeMap[editorNodeAsset.id] = copy;
                container.editorByRuntimeMap[copy.id] = editorNodeAsset.id;
                container.runtimeByEditorMap[editorNodeAsset.id] = copy.id;
            }

            onFinished.Invoke();
        }

        private static void FilterPort(IUniversalFlowNodeAsset universalFlowNodeAsset, List<FlowPortAsset> portAssets)
        {
            List<string> portIds = portAssets.Select((x) => x.portName).ToList();
            FlowShowOrHideUtility.FilterPort(universalFlowNodeAsset, portIds);

            for (int i = portAssets.Count - 1; i >= 0; i--)
            {
                FlowPortAsset portAsset = portAssets[i];
                if (portIds.Contains(portAsset.portName) == false) portAssets.RemoveAt(i);
            }
        }
    }
}