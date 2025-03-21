using System;
using System.Collections.Generic;
using Emilia.Reference;
using UnityEngine;

namespace Emilia.Flow
{
    [Serializable]
    public abstract class FlowNodeAsset
    {
        [SerializeField, HideInInspector]
        public readonly int id;

        [SerializeField, HideInInspector]
        private List<FlowPortAsset> _inputPorts = new List<FlowPortAsset>();

        [SerializeField, HideInInspector]
        private List<FlowPortAsset> _outputPorts = new List<FlowPortAsset>();

        public IReadOnlyList<FlowPortAsset> inputPorts => this._inputPorts;
        public IReadOnlyList<FlowPortAsset> outputPorts => this._outputPorts;

        protected FlowNodeAsset() { }
        public abstract FlowNode CreateNode();
    }

    [Serializable]
    public abstract class FlowNode : IReference
    {
        private FlowNodeAsset _flowNodeAsset;
        private FlowGraph _graph;

        private Dictionary<string, FlowPort> _inputPorts = new Dictionary<string, FlowPort>();
        private Dictionary<string, FlowPort> _outputPorts = new Dictionary<string, FlowPort>();

        protected Dictionary<string, Func<object>> getValueCache { get; } = new Dictionary<string, Func<object>>();
        protected Dictionary<string, Action<object>> methodCaches { get; } = new Dictionary<string, Action<object>>();

        public FlowNodeAsset flowNodeAsset => this._flowNodeAsset;
        public FlowGraph graph => this._graph;

        /// <summary>
        /// 所有InputPort，根据Id索引
        /// </summary>
        public IReadOnlyDictionary<string, FlowPort> inputPorts => this._inputPorts;

        /// <summary>
        /// 所有OutputPort，根据Id索引
        /// </summary>
        public IReadOnlyDictionary<string, FlowPort> outputPorts => this._outputPorts;

        public void OnInit(FlowNodeAsset flowNodeAsset, FlowGraph graph)
        {
            this._flowNodeAsset = flowNodeAsset;
            this._graph = graph;

            InitMethodCache();
            InitPorts();
            OnInit();
        }

        private void InitPorts()
        {
            this._inputPorts.Clear();
            this._outputPorts.Clear();

            int inputCount = this._flowNodeAsset.inputPorts.Count;
            for (int i = 0; i < inputCount; i++)
            {
                FlowPortAsset portAsset = this._flowNodeAsset.inputPorts[i];
                FlowPort port = ReferencePool.Acquire<FlowPort>();
                port.OnInit(portAsset, this);
                this._inputPorts.Add(portAsset.portName, port);
            }

            int outputCount = this._flowNodeAsset.outputPorts.Count;
            for (int i = 0; i < outputCount; i++)
            {
                FlowPortAsset portAsset = this._flowNodeAsset.outputPorts[i];
                FlowPort port = ReferencePool.Acquire<FlowPort>();
                port.OnInit(portAsset, this);
                this._outputPorts.Add(portAsset.portName, port);
            }
        }

        /// <summary>
        /// 获取InputPort的值
        /// </summary>
        protected T GetInputValue<T>(string portName, T defaultValue = default)
        {
            FlowPort port = this._inputPorts.GetValueOrDefault(portName);
            if (port == null || port.edges.Count == 0) return defaultValue;
            FlowEdge edge = port.edges[0];
            return edge.outputPort.GetValue<T>();
        }

        /// <summary>
        /// 调用OutputPort的函数
        /// </summary>
        protected void InvokeOutputPort(string portName, object arg = null)
        {
            FlowPort port = this._outputPorts.GetValueOrDefault(portName);
            if (port == null) return;
            int count = port.edges.Count;
            for (int i = 0; i < count; i++)
            {
                FlowEdge edge = port.edges[i];
                edge.inputPort.Invoke(arg);
            }
        }

        /// <summary>
        /// 获取OutputPort的值
        /// </summary>
        public T GetPortValue<T>(string portName)
        {
            return (T) getValueCache[portName]();
        }

        /// <summary>
        /// 获取OutputPort的函数
        /// </summary>
        public Action<object> GetPortAction(string portName)
        {
            return methodCaches.GetValueOrDefault(portName);
        }

        public void Dispose()
        {
            OnDispose();

            foreach (FlowPort port in this._inputPorts.Values) port.OnDispose();
            foreach (FlowPort port in this._outputPorts.Values) port.OnDispose();

            ReferencePool.Release(this);
        }

        void IReference.Clear()
        {
            getValueCache.Clear();
            methodCaches.Clear();
            
            this._inputPorts.Clear();
            this._outputPorts.Clear();
            this._flowNodeAsset = null;
            this._graph = null;
        }

        protected virtual void InitMethodCache() { }

        protected virtual void OnInit() { }
        protected virtual void OnDispose() { }
    }
}