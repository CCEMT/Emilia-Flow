using System;

namespace Emilia.Flow.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class FlowPortHideIfAttribute : Attribute
    {
        public string portId;
        public object value;

        public FlowPortHideIfAttribute(string portId, object value = null)
        {
            this.portId = portId;
            this.value = value;
        }
    }
}