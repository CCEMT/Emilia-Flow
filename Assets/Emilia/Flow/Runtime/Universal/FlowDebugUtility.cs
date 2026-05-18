#if UNITY_EDITOR
using System;
using System.Reflection;
using Emilia.Kit.Editor;
#endif

namespace Emilia.Flow.Emilia
{
    public static class FlowDebugUtility
    {
#if UNITY_EDITOR
        private static Type _editorDebugUtilityType;
        private static bool _editorDebugUtilityResolved;

        private static Type editorDebugUtilityType
        {
            get
            {
                if (_editorDebugUtilityResolved) return _editorDebugUtilityType;

                _editorDebugUtilityResolved = true;
                _editorDebugUtilityType = Assembly.Load("Emilia.Editor")?.GetType("Emilia.Flow.Editor.EditorFlowDebugUtility");

                return _editorDebugUtilityType;
            }
        }
#endif

        public static void SetState(FlowNode node, bool isDebug)
        {
#if UNITY_EDITOR
            if (node == null) return;
            ReflectUtility.Invoke(null, editorDebugUtilityType, nameof(SetState), new object[] {node, isDebug});
#endif
        }

        public static void Ping(FlowNode node, string message)
        {
#if UNITY_EDITOR
            if (node == null) return;
            ReflectUtility.Invoke(null, editorDebugUtilityType, nameof(Ping), new object[] {node, message});
#endif
        }
    }
}