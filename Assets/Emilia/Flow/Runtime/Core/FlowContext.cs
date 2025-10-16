using System;
using System.Threading;
using Emilia.Reference;

namespace Emilia.Flow
{
    /// <summary>
    /// 流程执行上下文
    /// </summary>
    public class FlowContext : IReference
    {
        private static readonly ThreadLocal<FlowContext> _current = new();
        
        /// <summary>
        /// 当前执行上下文（如果没有则返回null）
        /// </summary>
        public static FlowContext Current => _current.Value;
        
        /// <summary>
        /// 触发当前调用的边
        /// </summary>
        public FlowEdge SourceEdge { get; private set; }
        
        /// <summary>
        /// 触发当前调用的输出节点
        /// </summary>
        public FlowNode SourceNode { get; private set; }
        
        /// <summary>
        /// 触发当前调用的输出端口名
        /// </summary>
        public string SourcePortName { get; private set; }
        
        /// <summary>
        /// 生成唯一的边ID（节点ID_端口名）
        /// </summary>
        public string EdgeId => $"{SourceNode.flowNodeAsset.id}_{SourcePortName}";
        
        /// <summary>
        /// 进入新的执行上下文
        /// </summary>
        internal static IDisposable Enter(FlowNode sourceNode, FlowEdge edge, string portName)
        {
            FlowContext context = ReferencePool.Acquire<FlowContext>();
            context.SourceNode = sourceNode;
            context.SourceEdge = edge;
            context.SourcePortName = portName;
            
            _current.Value = context;
            return new Scope(context);
        }
        
        void IReference.Clear()
        {
            SourceNode = null;
            SourceEdge = null;
            SourcePortName = null;
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
                _current.Value = null;
                ReferencePool.Release(_context);
            }
        }
    }
}

