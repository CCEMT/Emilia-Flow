using System;

namespace Emilia.Flow.Attributes
{
    /// <summary>
    /// 端口颜色
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class,
        AllowMultiple = true)]
    public class FlowPortColorAttribute : Attribute
    {
        public string portId;
        public float r;
        public float g;
        public float b;

        public FlowPortColorAttribute(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public FlowPortColorAttribute(string portId, float r, float g, float b)
        {
            this.portId = portId;
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }
}
