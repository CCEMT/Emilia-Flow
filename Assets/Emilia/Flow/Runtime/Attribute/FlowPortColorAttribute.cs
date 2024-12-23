using System;

namespace Emilia.Flow.Attributes
{
    public class FlowPortColorAttribute : Attribute
    {
        public float r;
        public float g;
        public float b;

        public FlowPortColorAttribute(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }
}