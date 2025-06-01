using System;

namespace Emilia.Flow.Attributes
{
    /// <summary>
    /// 端口插入排序
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class FlowInsertOrderAttribute : Attribute
    {
        public int insertOrder;

        public FlowInsertOrderAttribute(int insertOrder)
        {
            this.insertOrder = insertOrder;
        }
    }
}