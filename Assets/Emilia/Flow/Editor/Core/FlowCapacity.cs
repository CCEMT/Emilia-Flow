using System;

namespace Emilia.Flow.Editor
{
    [Flags]
    public enum FlowCapacity
    {
        None = 0,

        /// <summary>
        /// 在节点上编辑属性（此功能使用IMGUI绘制，性能开销较大，保证节点数量不会过多时使用）
        /// </summary>
        EditToNode = 1,
    }
}