using System;

namespace Emilia.Flow.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class FlowPortShowIfAttribute : Attribute
    {
        public string portId;
        public object value;

        public FlowPortShowIfAttribute(string portId, object value = null)
        {
            this.portId = portId;
            this.value = value;
        }
    }
}