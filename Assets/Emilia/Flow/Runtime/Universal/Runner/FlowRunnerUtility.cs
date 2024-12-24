using System;
using System.Collections.Generic;
using System.Reflection;

namespace Emilia.Flow
{
    public static class FlowRunnerUtility
    {
        private static int maxId = 0;
        private static Queue<int> idPool = new Queue<int>();

        public static int GetId()
        {
            if (idPool.Count == 0) return maxId++;
            return idPool.Dequeue();
        }

        public static void RecycleId(int id)
        {
            idPool.Enqueue(id);
        }

        public static IFlowRunner CreateRunner()
        {
#if UNITY_EDITOR
            Type type = Assembly.Load("Emilia.Flow.Editor").GetType("Emilia.Flow.Editor.EditorFlowRunner");
            return Activator.CreateInstance(type) as IFlowRunner;
#else
            return new RuntimeFlowRunner();
#endif
        }
    }
}