using System.Reflection;
using Emilia.Flow.Attributes;

namespace Emilia.Flow.Editor
{
    public struct FlowPortIf
    {
        public MemberInfo memberInfo;
        public FlowPortShowIfAttribute showIfAttribute;
        public FlowPortHideIfAttribute hideIfAttribute;
    }
}