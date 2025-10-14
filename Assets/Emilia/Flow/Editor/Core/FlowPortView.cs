using System.Collections.Generic;
using System.Reflection;
using Emilia.Flow.Attributes;
using Emilia.Node.Editor;
using Emilia.Reflection.Editor;
using UnityEngine.UIElements;

namespace Emilia.Flow.Editor
{
    public class FlowPortView : EditorPortView
    {
        protected override void OnContextualMenuManipulator(ContextualMenuPopulateEvent evt)
        {
            if (IsRuntime()) OnRuntimeTrigger(evt);
            base.OnContextualMenuManipulator(evt);
        }

        protected virtual bool IsRuntime()
        {
            GetFlowRunnerEvent getFlowRunnerEvent = GetFlowRunnerEvent.GetPooled();
            getFlowRunnerEvent.target = graphView;

            graphView.SendEvent_Internal(getFlowRunnerEvent, DispatchMode_Internals.Immediate);

            return getFlowRunnerEvent.runner != null && getFlowRunnerEvent.runner.isActive;
        }

        protected virtual void OnRuntimeTrigger(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction($"Execute {info.displayName}", (_) => OnExecute());

            evt.menu.AppendSeparator();
        }

        protected virtual void OnExecute()
        {
            GetFlowRunnerEvent getFlowRunnerEvent = GetFlowRunnerEvent.GetPooled();
            getFlowRunnerEvent.target = graphView;

            graphView.SendEvent_Internal(getFlowRunnerEvent, DispatchMode_Internals.Immediate);

            EditorFlowRunner runner = getFlowRunnerEvent.runner;
            if (runner == null) return;

            EditorFlowAsset flowAsset = graphView.graphAsset as EditorFlowAsset;
            if (flowAsset == null) return;

            if (flowAsset.cacheRuntimeByEditorIdMap.TryGetValue(master.asset.id, out int runtimeId))
            {
                FlowNode runtimeNode = runner.graph.nodeById.GetValueOrDefault(runtimeId);

                MethodInfo[] methodInfos = runtimeNode.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                for (var i = 0; i < methodInfos.Length; i++)
                {
                    MethodInfo methodInfo = methodInfos[i];
                    if (methodInfo.Name != info.id) continue;

                    FlowMethodPort flowPortAttribute = methodInfo.GetCustomAttribute<FlowMethodPort>();
                    if (flowPortAttribute == null) break;

                    object[] parameters = new object[methodInfo.GetParameters().Length];
                    ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                    for (int j = 0; j < parameterInfos.Length; j++)
                    {
                        parameters[j] = parameterInfos[j].HasDefaultValue ? parameterInfos[j].DefaultValue : null;
                    }

                    methodInfo.Invoke(runtimeNode, parameters);
                }
            }
        }
    }
}