using System;

namespace Emilia.Flow.Attributes
{
    /// <summary>
    /// 端口排序
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class,
        AllowMultiple = true)]
    public class FlowPortOrderAttribute : Attribute
    {
        public string portId;
        public int order;

        public FlowPortOrderAttribute(int order)
        {
            this.order = order;
        }

        public FlowPortOrderAttribute(string portId, int order)
        {
            this.portId = portId;
            this.order = order;
        }
    }
}
