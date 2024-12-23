using System.Reflection;

namespace Emilia.Flow.Emilia
{
    public static class FlowDebugUtility
    {
        public static void SetState(FlowNode node, bool isDebug)
        {
#if UNITY_EDITOR
            Assembly.Load("Emilia.Flow.Editor").GetType("Emilia.Flow.Editor.EditorFlowDebugUtility").GetMethod("SetState").Invoke(null, new object[] {node, isDebug});
#endif
        }

        public static void Ping(FlowNode node, string message)
        {
#if UNITY_EDITOR
            Assembly.Load("Emilia.Flow.Editor").GetType("Emilia.Flow.Editor.EditorFlowDebugUtility").GetMethod("Ping").Invoke(null, new object[] {node, message});
#endif
        }
    }
}