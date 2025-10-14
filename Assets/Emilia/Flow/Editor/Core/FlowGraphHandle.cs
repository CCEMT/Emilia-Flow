using System.Collections.Generic;
using System.Linq;
using Emilia.Kit;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using UnityEditor;
using UnityEngine;

namespace Emilia.Flow.Editor
{
    [EditorHandle(typeof(EditorFlowAsset))]
    public class FlowGraphHandle : UniversalGraphHandle
    {
        private EditorGraphView editorGraphView;
        private EditorFlowAsset editorFlowAsset;
        private EditorFlowRunner debugRunner;
        private List<EditorFlowNodeView> displayNodeViews = new();

        public override void Initialize(EditorGraphView graphView)
        {
            base.Initialize(graphView);
            editorGraphView = graphView;
            editorFlowAsset = graphView.graphAsset as EditorFlowAsset;

            graphView.RegisterCallback<GetFlowRunnerEvent>(OnGetFlowRunnerEvent);
            graphView.RegisterCallback<SetFlowRunnerEvent>(OnSetFlowRunnerEvent);
        }

        public override void OnUpdate(EditorGraphView graphView)
        {
            base.OnUpdate(graphView);
            if (EditorApplication.isPlaying == false)
            {
                ClearRunner();
                return;
            }

            if (this.debugRunner == null)
            {
                List<EditorFlowRunner> runners = EditorFlowRunner.runnerByAssetId.GetValueOrDefault(graphView.graphAsset.id);
                if (runners != null && runners.Count == 1)
                {
                    this.debugRunner = runners.FirstOrDefault(runner => runner.isActive);
                    if (debugRunner != null && EditorFlowRunner.nodeMessage.TryGetValue(this.debugRunner.uid, out var queue)) queue.Clear();
                }
            }
            else
            {
                if (EditorFlowRunner.runnerByAssetId.ContainsKey(graphView.graphAsset.id) == false) ClearRunner();
                else if (EditorFlowRunner.runnerByAssetId[graphView.graphAsset.id].Contains(this.debugRunner) == false) ClearRunner();
                else if (debugRunner.isActive == false) ClearRunner();
            }

            DrawDebug();
        }

        private void OnGetFlowRunnerEvent(GetFlowRunnerEvent evt)
        {
            evt.runner = this.debugRunner;
        }

        private void OnSetFlowRunnerEvent(SetFlowRunnerEvent evt)
        {
            ClearRunner();
            this.debugRunner = evt.runner;

            if (EditorFlowRunner.nodeMessage.TryGetValue(this.debugRunner.uid, out var queue)) queue.Clear();
        }

        private void DrawDebug()
        {
            if (this.debugRunner == null)
            {
                ClearRunner();
                return;
            }

            FlowGraph flow = null;

            Queue<FlowGraph> queue = new();

            queue.Enqueue(this.debugRunner.graph);

            while (queue.Count > 0)
            {
                FlowGraph machine = queue.Dequeue();

                if (machine.graphAsset.id == editorGraphView.graphAsset.id)
                {
                    flow = machine;
                    break;
                }

                int childrenCount = machine.children.Count;
                for (var i = 0; i < childrenCount; i++)
                {
                    FlowGraph child = machine.children[i];
                    queue.Enqueue(child);
                }
            }

            if (flow == null)
            {
                ClearRunner();
                return;
            }

            if (flow.isActive == false)
            {
                ClearRunner();
                return;
            }

            SetState();
            ShowMessage();
        }

        private void SetState()
        {
            int uid = this.debugRunner.uid;
            if (EditorFlowRunner.nodeStates.TryGetValue(uid, out List<int> runningNodes) == false) return;

            List<EditorFlowNodeView> runningNodeViews = new();

            int runningNodeCount = runningNodes.Count;
            for (var i = 0; i < runningNodeCount; i++)
            {
                int nodeId = runningNodes[i];
                EditorFlowNodeView flowNodeView = GetEditorFlowNodeView(nodeId);
                if (flowNodeView == null) continue;

                runningNodeViews.Add(flowNodeView);
                if (displayNodeViews.Contains(flowNodeView)) continue;

                flowNodeView.SetFocus(Color.green);
                displayNodeViews.Add(flowNodeView);
            }

            for (var i = displayNodeViews.Count - 1; i >= 0; i--)
            {
                EditorFlowNodeView nodeView = displayNodeViews[i];
                if (nodeView == null) continue;

                if (runningNodeViews.Contains(nodeView)) continue;

                nodeView.ClearFocus();
                displayNodeViews.RemoveAt(i);
            }
        }

        private EditorFlowNodeView GetEditorFlowNodeView(int nodeId)
        {
            string editorNodeId = editorFlowAsset.cacheEditorByRuntimeIdMap.GetValueOrDefault(nodeId);
            if (string.IsNullOrEmpty(editorNodeId)) return null;

            IEditorNodeView nodeView = editorGraphView.graphElementCache.nodeViewById.GetValueOrDefault(editorNodeId);
            if (nodeView == null) return null;

            return nodeView as EditorFlowNodeView;
        }

        private void ShowMessage()
        {
            int uid = this.debugRunner.uid;

            EditorFlowRunner.nodeStates.TryGetValue(uid, out List<int> runningNodes);

            if (EditorFlowRunner.nodeMessage.TryGetValue(uid, out Queue<EditorFlowDebugPingMessage> messages) == false) return;

            while (messages.Count > 0)
            {
                EditorFlowDebugPingMessage message = messages.Dequeue();

                EditorFlowNodeView nodeView = GetEditorFlowNodeView(message.nodeId);
                if (nodeView == null) continue;

                nodeView.Tips(message.text);

                if (runningNodes.Contains(message.nodeId) == false) nodeView.SetFocus(Color.green, 1500);
            }
        }

        private void ClearRunner()
        {
            if (this.debugRunner == null) return;
            this.debugRunner = null;

            int nodeViewCount = editorGraphView.nodeViews.Count;
            for (var i = 0; i < nodeViewCount; i++)
            {
                IEditorNodeView nodeView = editorGraphView.nodeViews[i];
                if (nodeView == null) continue;

                EditorFlowNodeView flowNodeView = nodeView as EditorFlowNodeView;
                if (flowNodeView == null) continue;

                flowNodeView.ClearFocus();
            }

            this.displayNodeViews.Clear();
        }
    }
}