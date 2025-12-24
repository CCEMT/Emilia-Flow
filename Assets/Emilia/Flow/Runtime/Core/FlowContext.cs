using System;
using System.Collections.Generic;
using System.Threading;
using Emilia.Reference;

namespace Emilia.Flow
{
    /// <summary>
    /// 流程执行上下文
    /// </summary>
    public class FlowContext : IReference
    {
        private static readonly ThreadLocal<Stack<FlowContext>> _stack = new();

        /// <summary>
        /// 当前执行上下文（栈顶，如果栈为空则返回null）
        /// </summary>
        public static FlowContext current
        {
            get
            {
                Stack<FlowContext> stack = _stack.Value;
                if (stack == null || stack.Count == 0) return null;
                return stack.Peek();
            }
        }

        /// <summary>
        /// 触发当前调用的边
        /// </summary>
        public FlowEdge sourceEdge { get; private set; }

        /// <summary>
        /// 触发当前调用的输出节点
        /// </summary>
        public FlowNode sourceNode { get; private set; }

        /// <summary>
        /// 触发当前调用的输出端口名
        /// </summary>
        public string sourcePortName { get; private set; }

        /// <summary>
        /// 生成唯一的边ID（节点ID_端口名）
        /// </summary>
        public string EdgeId => $"{sourceNode.flowNodeAsset.id}_{sourcePortName}";

        /// <summary>
        /// 进入新的执行上下文
        /// </summary>
        internal static IDisposable Enter(FlowNode sourceNode, FlowEdge edge, string portName)
        {
            if (_stack.Value == null) _stack.Value = new Stack<FlowContext>();

            FlowContext context = ReferencePool.Acquire<FlowContext>();
            context.sourceNode = sourceNode;
            context.sourceEdge = edge;
            context.sourcePortName = portName;

            _stack.Value.Push(context);
            return new Scope(context);
        }

        void IReference.Clear()
        {
            sourceNode = null;
            sourceEdge = null;
            sourcePortName = null;
        }

        private class Scope : IDisposable
        {
            private readonly FlowContext _context;

            public Scope(FlowContext context)
            {
                _context = context;
            }

            public void Dispose()
            {
                Stack<FlowContext> stack = _stack.Value;
                if (stack != null && stack.Count > 0)
                {
                    FlowContext popped = stack.Pop();
                    ReferencePool.Release(popped);
                }
            }
        }
    }
}