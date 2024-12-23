using System;

namespace Emilia.Flow.Attributes
{
    [Flags]
    public enum FlowPortCapacity
    {
        None = 0,
        MultiConnect = 1,
        Vertical = 2,
        Expand = 4,
        ExpandInImGUI = 8,
    }

    public abstract class FlowPortGenerator : Attribute
    {
        public string displayName { get; private set; }
        public FlowPortCapacity capacity { get; private set; }

        public FlowPortGenerator(string displayName, FlowPortCapacity capacity = FlowPortCapacity.None)
        {
            this.displayName = displayName;
            this.capacity = capacity;
        }
    }

    public abstract class FlowValuePort : FlowPortGenerator
    {
        protected FlowValuePort(string displayName, FlowPortCapacity capacity = FlowPortCapacity.None) : base(displayName, capacity) { }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class FlowInputValuePort : FlowValuePort
    {
        public FlowInputValuePort(string displayName, FlowPortCapacity capacity = FlowPortCapacity.None) : base(displayName, capacity) { }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class FlowOutputValuePort : FlowValuePort
    {
        public FlowOutputValuePort(string displayName, FlowPortCapacity capacity = FlowPortCapacity.None) : base(displayName, capacity) { }
    }

    public abstract class FlowMethodPort : FlowPortGenerator
    {
        protected FlowMethodPort(string displayName, FlowPortCapacity capacity = FlowPortCapacity.None) : base(displayName, capacity) { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class FlowInputMethodPort : FlowMethodPort
    {
        public FlowInputMethodPort(string displayName, FlowPortCapacity capacity = FlowPortCapacity.None) : base(displayName, capacity) { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class FlowOutputMethodPort : FlowMethodPort
    {
        public FlowOutputMethodPort(string displayName, FlowPortCapacity capacity = FlowPortCapacity.None) : base(displayName, capacity) { }
    }
}