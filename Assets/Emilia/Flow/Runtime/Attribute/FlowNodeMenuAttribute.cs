using System;

namespace Emilia.Flow.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FlowNodeMenuAttribute : Attribute
    {
        public string path;
        public int priority;

        public FlowNodeMenuAttribute(string path, int priority = 0)
        {
            this.path = path;
            this.priority = priority;
        }
    }
}