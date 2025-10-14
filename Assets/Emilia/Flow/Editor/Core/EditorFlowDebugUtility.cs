using System.Collections.Generic;

namespace Emilia.Flow.Editor
{
    public struct EditorFlowDebugPingMessage
    {
        public int nodeId;
        public string text;
    }

    public static class EditorFlowDebugUtility
    {
        public static void SetState(FlowNode node, bool isDebug)
        {
            if (node.graph == null) return;
            if (node.graph.isActive == false) return;

            int uid = node.graph.uid;
            if (EditorFlowRunner.nodeStates.ContainsKey(uid) == false) EditorFlowRunner.nodeStates[uid] = new List<int>();

            bool isContains = EditorFlowRunner.nodeStates[uid].Contains(node.flowNodeAsset.id);

            if (isDebug)
            {
                if (isContains == false) EditorFlowRunner.nodeStates[uid].Add(node.flowNodeAsset.id);
            }
            else
            {
                if (isContains) EditorFlowRunner.nodeStates[uid].Remove(node.flowNodeAsset.id);
            }
        }

        public static void Ping(FlowNode node, string message)
        {
            if (node.graph == null) return;
            if (node.graph.isActive == false) return;

            EditorFlowDebugPingMessage pingMessage = new();
            pingMessage.nodeId = node.flowNodeAsset.id;
            pingMessage.text = message;

            int uid = node.graph.uid;

            if (EditorFlowRunner.nodeMessage.ContainsKey(node.graph.uid) == false) EditorFlowRunner.nodeMessage[uid] = new Queue<EditorFlowDebugPingMessage>();
            EditorFlowRunner.nodeMessage[uid].Enqueue(pingMessage);
        }
    }
}