using System;

namespace Emilia.Flow.Attributes
{
    /// <summary>
    /// 端口排序
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class FlowPortOrderAttribute : Attribute
    {
        public int order;

        public FlowPortOrderAttribute(int order)
        {
            this.order = order;
        }
    }
}